using EODService.DTOs.EOD;
using EODService.Logging;
using EODService.Models.Provider;
using EODService.Persistance;
using EODService.Persistance.Repo;
using EODSettingsApp.AppSettingsConfig;
using EODSettingsApp.ExternalConfig;
using EODSettingsApp.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    public partial class SettingsForm : Form
    {
        // ── Log Monitoring ──────────────────────────────────────────────────────

        private readonly System.Windows.Forms.Timer _logPollTimer = new();
        private long _lastLogFilePosition = 0;
        private bool _isPollingLog = false;


        private bool _isSplitterInitialized = false;

        // ── Constructor ─────────────────────────────────────────────────────────

        public SettingsForm()
        {
            InitializeComponent();
            AppIconHelper.ApplyAppIconAndTitle(this);

            SetupGridColumns();

            Load += async (_, _) => await SettingsForm_LoadAsync();

            InitializeBackgroundLogAndGridMonitoring();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            InitializeSplitterDistance();
        }

        private void InitializeSplitterDistance()
        {
            if (_isSplitterInitialized) return;

            if (splitMain != null && splitMain.Width > (splitMain.Panel1MinSize + splitMain.Panel2MinSize + splitMain.SplitterWidth))
            {
                int availableWidth = splitMain.Width;
                int minimum = splitMain.Panel1MinSize;
                int maximum = availableWidth - splitMain.Panel2MinSize - splitMain.SplitterWidth;
                int desired = (int)(availableWidth * 0.29); // ~29% for Left Execution Logs panel

                int targetDistance = Math.Max(minimum, Math.Min(desired, maximum));

                if (targetDistance >= minimum && targetDistance <= maximum)
                {
                    splitMain.SplitterDistance = targetDistance;
                    _isSplitterInitialized = true;
                }
            }
        }


        // ── Startup: Load Settings + Providers + Grid ───────────────────────────

        private async Task LoadCurrentSettingsAsync()
        {
            try
            {
                var connectionString = ConnectionStringResolver.Get();

                if (!string.IsNullOrWhiteSpace(connectionString) && !connectionString.Contains("YOUR_DB_USER"))
                {
                    // 1. Load all providers from database
                    await LoadProvidersAsync();

                    // 4. Load existing EOD records
                    await RefreshGridFromDatabaseAsync();
                }
                else
                {
                    SetStatus("ℹ Database not yet configured. Click 'Settings' to enter Oracle connection details.", success: true);
                }

                if (IsDisposed) return;

                // 2. Load active provider ID from external JSON
                var extSettings =
                    ExternalSettingsService.Load();

                var activeProviderId =
                    extSettings.ProviderSettings.ActiveProvider;


                // Select the provider matching the saved ID
                if (activeProviderId > 0 && cmbProvider.Items.Count > 0)
                {
                    cmbProvider.SelectedValue =
                        activeProviderId;
                }
                else if (cmbProvider.Items.Count > 0)
                {
                    cmbProvider.SelectedIndex = 0;
                }


                // 3. Load schedule settings
                var appSettings =
                    AppSettingsService.Load();

                PopulateScheduleUI(
                    appSettings.ScheduleSettings);
            }
            catch (Exception ex)
            {
                SetStatus(
                    $"Warning: Could not load full settings ({ex.Message})",
                    success: false);
            }
        }


        // ── Load Providers from Database ────────────────────────────────────────

        private async Task LoadProvidersAsync()
        {
            var connectionString = ConnectionStringResolver.Get();
            if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_DB_USER"))
                return;

            try
            {
                using var dbContext = AppDbContextFactory.Create(connectionString);
                var providerRepo = new ProviderRepo(dbContext);
                var providers = await providerRepo.GetAllProvidersAsync();

                if (IsDisposed) return;

                if (providers != null)
                {
                    // Only show providers that are supported by the service (defined in ProviderIds enum).
                    // This prevents legacy or internal-only DB rows from appearing in the selector.
                    var supportedIds = Enum.GetValues<EODService.Models.ProviderIds>()
                                          .Select(id => (int)id)
                                          .ToHashSet();

                    providers = providers
                        .Where(p => supportedIds.Contains(p.Id))
                        .ToList();
                }

                if (providers == null || !providers.Any())
                {
                    cmbProvider.DataSource = null;

                    SetStatus(
                        "No providers were found in the database.",
                        success: false);

                    return;
                }

                // Clear previous binding
                cmbProvider.DataSource = null;

                // Bind Provider objects
                cmbProvider.DataSource = providers;

                // What user sees
                cmbProvider.DisplayMember =
                    nameof(Provider.Name);

                // Actual value
                cmbProvider.ValueMember =
                    nameof(Provider.Id);
            }
            catch (Exception ex)
            {
                SetStatus(
                    $"Failed to load providers: {ex.Message}",
                    success: false);

                AppendLogError(
                    $"Provider loading error: {ex.Message}");
            }
        }


        // ── Populate Schedule UI ────────────────────────────────────────────────

        private void PopulateScheduleUI(
            ScheduleSettingsSection schedule)
        {
            chkEnableSchedule.CheckedChanged -=
                ChkEnableSchedule_CheckedChanged;

            chkEnableSchedule.Checked =
                schedule.Enabled;

            ToggleScheduleControlsState(
                schedule.Enabled);

            var days =
                schedule.WorkingDays ??
                new List<string>();

            chkMon.Checked =
                days.Contains(
                    "Monday",
                    StringComparer.OrdinalIgnoreCase);

            chkTue.Checked =
                days.Contains(
                    "Tuesday",
                    StringComparer.OrdinalIgnoreCase);

            chkWed.Checked =
                days.Contains(
                    "Wednesday",
                    StringComparer.OrdinalIgnoreCase);

            chkThu.Checked =
                days.Contains(
                    "Thursday",
                    StringComparer.OrdinalIgnoreCase);

            chkFri.Checked =
                days.Contains(
                    "Friday",
                    StringComparer.OrdinalIgnoreCase);

            chkSat.Checked =
                days.Contains(
                    "Saturday",
                    StringComparer.OrdinalIgnoreCase);

            chkSun.Checked =
                days.Contains(
                    "Sunday",
                    StringComparer.OrdinalIgnoreCase);


            if (TimeSpan.TryParse(
                schedule.RunTime,
                out var runTime))
            {
                dtpRunTime.Value =
                    DateTime.Today.Add(runTime);
            }
            else
            {
                dtpRunTime.Value =
                    DateTime.Today.AddHours(18);
            }


            UpdateNextRunIndicator(schedule);

            chkEnableSchedule.CheckedChanged +=
                ChkEnableSchedule_CheckedChanged;
        }


        // ── Enable / Disable Schedule ───────────────────────────────────────────

        private void ChkEnableSchedule_CheckedChanged(
            object? sender,
            EventArgs e)
        {
            bool isEnabled =
                chkEnableSchedule.Checked;

            ToggleScheduleControlsState(isEnabled);

            try
            {
                var appSettings =
                    AppSettingsService.Load();

                appSettings.ScheduleSettings.Enabled =
                    isEnabled;

                appSettings.ScheduleSettings.WorkingDays =
                    CollectSelectedWorkingDays();

                appSettings.ScheduleSettings.RunTime =
                    dtpRunTime.Value
                        .ToString("HH:mm:ss");

                AppSettingsService.Save(appSettings);


                var exePath =
                    EodServiceLauncher.ResolveExePath();

                WindowsTaskSchedulerService
                    .RegisterOrUpdateTask(
                        appSettings.ScheduleSettings,
                        exePath);


                UpdateNextRunIndicator(
                    appSettings.ScheduleSettings);

                SetStatus(
                    $"✔ Automated schedule " +
                    $"{(isEnabled ? "enabled" : "disabled")}.",
                    success: true);

                AppendLog(
                    $"[Schedule] Auto-schedule " +
                    $"{(isEnabled ? "enabled" : "disabled")}.");
            }
            catch (Exception ex)
            {
                SetStatus(
                    $"✘ Error updating schedule: {ex.Message}",
                    success: false);
            }
        }


        private void ToggleScheduleControlsState(
            bool isEnabled)
        {
            lblWorkingDaysLabel.Enabled = isEnabled;

            chkMon.Enabled = isEnabled;
            chkTue.Enabled = isEnabled;
            chkWed.Enabled = isEnabled;
            chkThu.Enabled = isEnabled;
            chkFri.Enabled = isEnabled;
            chkSat.Enabled = isEnabled;
            chkSun.Enabled = isEnabled;

            lblTimeLabel.Enabled = isEnabled;
            dtpRunTime.Enabled = isEnabled;

            btnSaveSchedule.Enabled = isEnabled;
            btnRunNow.Enabled = isEnabled;
        }


        // ── DataGridView Columns ────────────────────────────────────────────────

        private void SetupGridColumns()
        {
            dgvResults.Columns.Clear();

            dgvResults.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Id",
                    HeaderText = "Stock ID",
                    DataPropertyName = "Id"
                });

            dgvResults.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Name",
                    HeaderText = "Stock Name",
                    DataPropertyName = "Name"
                });

            dgvResults.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Date",
                    HeaderText = "Date",
                    DataPropertyName = "Date"
                });

            dgvResults.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Open",
                    HeaderText = "Open",
                    DataPropertyName = "Open"
                });

            dgvResults.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "High",
                    HeaderText = "High",
                    DataPropertyName = "High"
                });

            dgvResults.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Low",
                    HeaderText = "Low",
                    DataPropertyName = "Low"
                });

            dgvResults.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Close",
                    HeaderText = "Close",
                    DataPropertyName = "Close"
                });

            dgvResults.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "AdjustedClose",
                    HeaderText = "Adj. Close",
                    DataPropertyName = "AdjustedClose"
                });

            dgvResults.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Volume",
                    HeaderText = "Volume",
                    DataPropertyName = "Volume"
                });
        }


        // ── Save Schedule ───────────────────────────────────────────────────────

        private void BtnSaveSchedule_Click(
            object? sender,
            EventArgs e)
        {
            if (cmbProvider.SelectedValue == null)
            {
                MessageBox.Show(
                    "Please select an active provider.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            // Get ID from ComboBox
            var selectedProviderId =
                Convert.ToInt32(
                    cmbProvider.SelectedValue);


            // Get Name only for UI/logging
            var selectedProviderName =
                cmbProvider.Text;


            var selectedDays =
                CollectSelectedWorkingDays();


            if (chkEnableSchedule.Checked &&
                selectedDays.Count == 0)
            {
                MessageBox.Show(
                    "Please select at least one working day.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            try
            {
                // ── 1. Save ActiveProviderId to External JSON ───────────────────

                var externalSettings =
                    ExternalSettingsService.Load();

                externalSettings.ProviderSettings.ActiveProvider =
                    selectedProviderId;

                ExternalSettingsService.Save(
                    externalSettings);


                // ── 2. Build Schedule Settings ──────────────────────────────────

                var scheduleSection =
                    new ScheduleSettingsSection
                    {
                        Enabled =
                            chkEnableSchedule.Checked,

                        WorkingDays =
                            selectedDays,

                        RunTime =
                            dtpRunTime.Value
                                .ToString("HH:mm:ss")
                    };


                // ── 3. Save AppSettings ─────────────────────────────────────────

                var appSettings =
                    AppSettingsService.Load();

                appSettings.ScheduleSettings =
                    scheduleSection;

                AppSettingsService.Save(
                    appSettings);


                // ── 4. Register Windows Task ────────────────────────────────────

                var exePath =
                    EodServiceLauncher.ResolveExePath();

                WindowsTaskSchedulerService
                    .RegisterOrUpdateTask(
                        scheduleSection,
                        exePath);


                // ── 5. Update UI ────────────────────────────────────────────────

                UpdateNextRunIndicator(
                    scheduleSection);

                SetStatus(
                    $"✔ Schedule saved & Windows Task updated " +
                    $"(Provider: {selectedProviderName}).",
                    success: true);

                AppendLog(
                    $"[Schedule] Settings saved successfully. " +
                    $"Provider ID: {selectedProviderId}.");
            }
            catch (Exception ex)
            {
                SetStatus(
                    $"✘ Save schedule failed: {ex.Message}",
                    success: false);

                AppendLogError(
                    $"Save schedule error: {ex.Message}");

                MessageBox.Show(
                    $"Failed to save schedule settings:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ── Instant Manual Run ──────────────────────────────────────────────────

        private async void BtnRunNow_Click(object? sender, EventArgs e)
        {
            if (EodServiceLauncher.IsRunning())
            {
                MessageBox.Show(
                    "An instance of EODService is already running. Please wait for it to complete before starting a new import.",
                    "Service Currently Running",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Auto-save currently selected active provider prior to execution
                if (cmbProvider.SelectedValue != null)
                {
                    var selectedProviderId = Convert.ToInt32(cmbProvider.SelectedValue);
                    var externalSettings = ExternalSettingsService.Load();
                    externalSettings.ProviderSettings.ActiveProvider = selectedProviderId;
                    ExternalSettingsService.Save(externalSettings);
                }

                btnRunNow.Enabled = false;
                btnRunNow.Text = "⏳ Running...";

                SetStatus("Launching instant EOD service data import...", success: true);
                AppendLog("================================================================================");
                AppendLog($"[Manual Run] Instant EOD import execution requested at {DateTime.Now:HH:mm:ss}");

                var exePath = EodServiceLauncher.ResolveExePath();
                var process = EodServiceLauncher.Launch(exePath);

                SetStatus("⏳ Instant EOD import running in background...", success: true);
                AppendLog($"[Manual Run] Launched process ID: {process.Id}. Waiting for completion...");

                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    SetStatus("✔ Instant EOD import completed successfully.", success: true);
                    AppendLog($"[Manual Run] Process ID {process.Id} completed successfully with exit code 0.");
                    await RefreshGridFromDatabaseAsync();
                }
                else
                {
                    SetStatus($"✘ Instant EOD import failed with exit code {process.ExitCode}.", success: false);
                    AppendLogError($"[Manual Run] Process ID {process.Id} exited with error code {process.ExitCode}.");
                    MessageBox.Show(
                        $"EOD data import process exited with error code {process.ExitCode}. Please check the log view for details.",
                        "Execution Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                SetStatus($"✘ Instant run failed: {ex.Message}", success: false);
                AppendLogError($"Manual run error: {ex.Message}");
                MessageBox.Show(
                    $"Could not launch instant EOD data import:\n\n{ex.Message}",
                    "Execution Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnRunNow.Text = "⚡ Run Now";
                btnRunNow.Enabled = true;
            }
        }




        // ── Next Run Calculation ────────────────────────────────────────────────

        private void UpdateNextRunIndicator(
            ScheduleSettingsSection schedule)
        {
            var nextRun =
                CalculateNextRunTime(schedule);

            var lastRunInfo =
                LastRunStatusHelper.GetLastRunInfo();

            string nextRunText;
            if (nextRun.HasValue)
            {
                nextRunText = $"🕒 Next Run: {nextRun.Value:ddd, MMM d 'at' HH:mm}";
            }
            else
            {
                nextRunText = "🕒 Next Run: Schedule Disabled";
            }

            lblNextRunStatus.ForeColor = lastRunInfo.IsSuccess
                ? Color.FromArgb(22, 101, 52)
                : (lastRunInfo.HasRun ? Color.FromArgb(185, 28, 28) : Color.FromArgb(30, 58, 138));

            lblNextRunStatus.Text = $"{lastRunInfo.SummaryText}   │   {nextRunText}";
        }


        private DateTime? CalculateNextRunTime(
            ScheduleSettingsSection settings)
        {
            if (!settings.Enabled ||
                settings.WorkingDays == null ||
                !settings.WorkingDays.Any())
            {
                return null;
            }

            if (!TimeSpan.TryParse(
                settings.RunTime,
                out var runTime))
            {
                return null;
            }

            var now =
                DateTime.Now;

            for (int i = 0; i < 7; i++)
            {
                var candidateDate =
                    now.Date.AddDays(i);

                var candidateDateTime =
                    candidateDate.Add(runTime);

                if (settings.WorkingDays.Contains(
                    candidateDate.DayOfWeek.ToString(),
                    StringComparer.OrdinalIgnoreCase))
                {
                    if (candidateDateTime > now)
                    {
                        return candidateDateTime;
                    }
                }
            }

            return null;
        }


        private List<string> CollectSelectedWorkingDays()
        {
            var days =
                new List<string>();

            if (chkMon.Checked)
                days.Add("Monday");

            if (chkTue.Checked)
                days.Add("Tuesday");

            if (chkWed.Checked)
                days.Add("Wednesday");

            if (chkThu.Checked)
                days.Add("Thursday");

            if (chkFri.Checked)
                days.Add("Friday");

            if (chkSat.Checked)
                days.Add("Saturday");

            if (chkSun.Checked)
                days.Add("Sunday");

            return days;
        }


        // ── Background Log Monitoring ───────────────────────────────────────────

        private void InitializeBackgroundLogAndGridMonitoring()
        {
            var logPath =
                FileLoggerProvider.GetTodayLogFilePath();

            if (File.Exists(logPath))
            {
                try
                {
                    using var fs = new FileStream(
                        logPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite);

                    using var reader = new StreamReader(fs);
                    string initialContent = reader.ReadToEnd();

                    if (!string.IsNullOrWhiteSpace(initialContent))
                    {
                        AppendLog(initialContent.TrimEnd());
                    }

                    _lastLogFilePosition = fs.Length;
                }
                catch
                {
                    _lastLogFilePosition = 0;
                }
            }
            else
            {
                _lastLogFilePosition = 0;
            }

            _logPollTimer.Interval = 2000;

            _logPollTimer.Tick += async (s, e) =>
                await PollLogFileAndRefreshGridAsync();

            _logPollTimer.Start();

            _ = PollLogFileAndRefreshGridAsync();
        }


        private async Task PollLogFileAndRefreshGridAsync()
        {
            if (_isPollingLog) return;
            _isPollingLog = true;

            try
            {
                var logPath = FileLoggerProvider.GetTodayLogFilePath();

                if (!File.Exists(logPath))
                    return;

                using var fs =
                    new FileStream(
                        logPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite);

                if (fs.Length < _lastLogFilePosition)
                {
                    _lastLogFilePosition = 0;
                }


                if (fs.Length > _lastLogFilePosition)
                {
                    fs.Seek(
                        _lastLogFilePosition,
                        SeekOrigin.Begin);

                    using var reader =
                        new StreamReader(fs);

                    string? newContent =
                        await reader.ReadToEndAsync();

                    _lastLogFilePosition =
                        fs.Position;


                    if (!string.IsNullOrWhiteSpace(
                        newContent))
                    {
                        AppendLog(
                            newContent.TrimEnd());


                        if (newContent.Contains(
                                "completed successfully") ||
                            newContent.Contains(
                                "EOD import complete"))
                        {
                            await RefreshGridFromDatabaseAsync();

                            var updatedSettings =
                                AppSettingsService.Load();

                            UpdateNextRunIndicator(
                                updatedSettings.ScheduleSettings);
                        }
                    }
                }
            }
            catch
            {
                // Best effort log reading
            }
            finally
            {
                _isPollingLog = false;
            }
        }


        // ── Refresh Grid using Short-Lived DbContext ────────────────────────────

        private async Task RefreshGridFromDatabaseAsync()
        {
            var connectionString = ConnectionStringResolver.Get();
            if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_DB_USER"))
                return;

            try
            {
                using var dbContext = AppDbContextFactory.Create(connectionString);
                var dailyRecords =
                    await dbContext.EodDaily
                        .AsNoTracking()
                        .ToListAsync();

                if (IsDisposed) return;

                if (dailyRecords != null &&
                    dailyRecords.Any())
                {
                    if (InvokeRequired)
                    {
                        Invoke(() =>
                            PopulateDailyRecords(
                                dailyRecords));
                    }
                    else
                    {
                        PopulateDailyRecords(
                            dailyRecords);
                    }
                }
                else
                {
                    dgvResults.Rows.Clear();

                    lblGridTitle.Text =
                        "EOD Results " +
                        "(Automated Service Operations) — " +
                        "0 record(s)";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(
                    "[SettingsForm] " +
                    "RefreshGridFromDatabaseAsync error: " +
                    ex.Message);
            }
        }


        // ── Populate Grid ───────────────────────────────────────────────────────

        private void PopulateDailyRecords(
            IEnumerable<EodData> dailyRecords)
        {
            dgvResults.Rows.Clear();

            foreach (var r in dailyRecords)
            {
                dgvResults.Rows.Add(
                    r.Id,
                    r.Name,
                    r.Date.ToString("yyyy-MM-dd"),
                    r.Open?.ToString("F4") ?? "-",
                    r.High?.ToString("F4") ?? "-",
                    r.Low?.ToString("F4") ?? "-",
                    r.Close?.ToString("F4") ?? "-",
                    r.AdjustedClose?.ToString("F4") ?? "-",
                    r.Volume?.ToString("N0") ?? "-"
                );
            }

            lblGridTitle.Text =
                $"EOD Results " +
                $"(Automated Service Operations) — " +
                $"{dailyRecords.Count()} record(s)";
        }


        // ── Existing PopulateGrid Helper ────────────────────────────────────────

        public void PopulateGrid(
            IEnumerable<EodData> results)
        {
            PopulateDailyRecords(results);
        }


        // ── Logging Helpers ─────────────────────────────────────────────────────

        private void AppendLog(string message)
        {
            if (rtbLogs.IsDisposed)
                return;

            if (InvokeRequired)
            {
                Invoke(() => AppendLog(message));
                return;
            }

            rtbLogs.AppendText(
                $"{message}{Environment.NewLine}");

            rtbLogs.SelectionStart =
                rtbLogs.Text.Length;

            rtbLogs.ScrollToCaret();
        }


        private void AppendLogError(string message)
        {
            if (rtbLogs.IsDisposed)
                return;

            if (InvokeRequired)
            {
                Invoke(() => AppendLogError(message));
                return;
            }

            rtbLogs.SelectionColor =
                Color.Red;

            rtbLogs.AppendText(
                $"ERROR: {message}{Environment.NewLine}");

            rtbLogs.SelectionColor =
                rtbLogs.ForeColor;

            rtbLogs.SelectionStart =
                rtbLogs.Text.Length;

            rtbLogs.ScrollToCaret();
        }


        private void SetStatus(
            string message,
            bool success)
        {
            lblStatus.ForeColor =
                success
                    ? Color.FromArgb(22, 163, 74)
                    : Color.FromArgb(185, 28, 28);

            lblStatus.Text =
                message;
        }

        // ── Menu handlers ────────────────────────────────────────────────────────
        private async void MnuItemSettings_Click(object? sender, EventArgs e)
        {
            using var form = new HierarchicalSettingsForm();
            form.ShowDialog(this);
            await LoadCurrentSettingsAsync();
        }

        /// <summary>
        /// Menu → History: opens the historical stock price explorer & CSV exporter dialog.
        /// </summary>
        private void MnuItemHistoricalData_Click(object? sender, EventArgs e)
        {
            using var form = new HistoricalDataForm();
            form.ShowDialog(this);
        }
        // Loading the Form
        private async Task SettingsForm_LoadAsync()
        {
            try
            {
                SetStatus("Connecting to database...", false);

                await InitializeDatabaseAsync();

                SetStatus("Loading settings...", false);

                await LoadCurrentSettingsAsync();

                SetStatus("Loaded successfully...", true);

            }
            catch (Exception ex)
            {
                SetStatus(
                    $"Warning: Could not load settings ({ex.Message})",
                    false);
            }
        }

        // -- Initialization: db
        private async Task InitializeDatabaseAsync()
        {
            var connectionString = ConnectionStringResolver.Get();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured.");
            }

            using var dbContext = AppDbContextFactory.Create(connectionString);
            await dbContext.Database.EnsureCreatedAsync();
            await DatabaseSeeder.SeedAsync(dbContext);
        }

        // ── Provider ComboBox ───────────────────────────────────────────────────

        private void cmbProvider_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            // No action needed.
            //
            // The selected provider ID is saved when
            // BtnSaveSchedule is clicked.
        }


        // ── Navigation Event Handlers ──────────────────────────────────────────

        private void MnuItemUserGuide_Click(object? sender, EventArgs e)
        {
            using var userGuide = new UserGuideForm();
            userGuide.ShowDialog(this);
        }

        // ── Cleanup ─────────────────────────────────────────────────────────────

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            _logPollTimer.Stop();
            _logPollTimer.Dispose();

            base.OnFormClosed(e);
        }
    }
}
