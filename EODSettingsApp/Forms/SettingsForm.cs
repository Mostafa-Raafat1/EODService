using EODService.DTOs.EOD;
using EODService.Logging;
using EODService.Models.Provider;
using EODService.Persistance;
using EODService.Persistance.Repo;
using EODService.Services;
using EODSettingsApp.AppSettingsConfig;
using EODSettingsApp.ExternalConfig;
using EODSettingsApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        // ── Database ────────────────────────────────────────────────────────────

        private readonly AppDbContext _dbContext;
        private readonly IProvider _providerRepo;


        // ── Log Monitoring ──────────────────────────────────────────────────────

        private readonly System.Windows.Forms.Timer _logPollTimer = new();
        private long _lastLogFilePosition = 0;


        // ── Constructor ─────────────────────────────────────────────────────────

        public SettingsForm()
        {
            InitializeComponent();

            SetupGridColumns();

            // ── Create ONE database connection for this form ────────────────────

            var appSettingsPath = AppSettingsPath.Resolve();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(appSettingsPath)!)
                .AddJsonFile(
                    Path.GetFileName(appSettingsPath),
                    optional: false,
                    reloadOnChange: false)
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString) ||
                connectionString.Contains("YOUR_DB_USER"))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured.");
            }

            _dbContext =
                AppDbContextFactory.Create(connectionString);

            // ── Create repositories using the same DbContext ────────────────────

            _providerRepo =
                new ProviderRepo(_dbContext);


            // ── Load settings asynchronously ────────────────────────────────────

            _ = LoadCurrentSettingsAsync();


            // ── Start background log monitoring ─────────────────────────────────

            InitializeBackgroundLogAndGridMonitoring();
        }


        // ── Startup: Load Settings + Providers + Grid ───────────────────────────

        private async Task LoadCurrentSettingsAsync()
        {
            try
            {
                // 1. Load all providers from database
                await LoadProvidersAsync();


                // 2. Load active provider ID from external JSON
                var extSettings =
                    ExternalSettingsService.Load();

                var activeProviderId =
                    extSettings.ProviderSettings.ActiveProvider;


                // Select the provider matching the saved ID
                if (activeProviderId > 0)
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


                // 4. Load existing EOD records
                await RefreshGridFromDatabaseAsync();
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
            try
            {
                var providers =
                    await _providerRepo.GetAllProvidersAsync();

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


        // ── Next Run Calculation ────────────────────────────────────────────────

        private void UpdateNextRunIndicator(
            ScheduleSettingsSection schedule)
        {
            var nextRun =
                CalculateNextRunTime(schedule);

            if (nextRun.HasValue)
            {
                lblNextRunStatus.ForeColor =
                    Color.FromArgb(30, 58, 138);

                lblNextRunStatus.Text =
                    $"🕒 Next Scheduled Run: " +
                    $"{nextRun.Value:dddd, MMM d, yyyy 'at' HH:mm}";
            }
            else
            {
                lblNextRunStatus.ForeColor =
                    Color.FromArgb(185, 28, 28);

                lblNextRunStatus.Text =
                    "🕒 Next Scheduled Run: " +
                    "Automated schedule is disabled or has no working days selected.";
            }
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
                _lastLogFilePosition =
                    new FileInfo(logPath).Length;
            }

            _logPollTimer.Interval = 2000;

            _logPollTimer.Tick += async (s, e) =>
                await PollLogFileAndRefreshGridAsync();

            _logPollTimer.Start();

            _ = PollLogFileAndRefreshGridAsync();
        }


        private async Task PollLogFileAndRefreshGridAsync()
        {
            var logPath =
                FileLoggerProvider.GetTodayLogFilePath();

            if (!File.Exists(logPath))
                return;

            try
            {
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
        }


        // ── Refresh Grid using Shared DbContext ─────────────────────────────────

        private async Task RefreshGridFromDatabaseAsync()
        {
            try
            {
                var dailyRecords =
                    await _dbContext.EodDaily
                        .AsNoTracking()
                        .ToListAsync();


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


        // ── Menu Handlers ───────────────────────────────────────────────────────

        private void MnuItemProviderSettings_Click(
            object? sender,
            EventArgs e)
        // ── Menu handlers ────────────────────────────────────────────────────────
        private void MnuItemSettings_Click(object? sender, EventArgs e)
        {
            using var form =
                new ProviderSettingsForm();

            form.ShowDialog(this);

            // Reload providers in case they were changed
            _ = LoadProvidersAsync();
        }


        private void MnuItemSymbolSettings_Click(
            object? sender,
            EventArgs e)
        {
            using var form =
                new SymbolSettingsForm();

            form.ShowDialog(this);
        }


        private void MnuItemDatabaseSettings_Click(
            object? sender,
            EventArgs e)
        {
            using var form =
                new DatabaseSettingsForm();

            using var form = new HierarchicalSettingsForm();
            form.ShowDialog(this);
        }


        private void MnuItemHistoricalData_Click(
            object? sender,
            EventArgs e)
        {
            using var form =
                new HistoricalDataForm();

            form.ShowDialog(this);
        }


        private void MnuItemAddStock_Click(
            object? sender,
            EventArgs e)
        /// <summary>
        /// Settings → Historical Data: opens the historical stock price explorer & CSV exporter dialog.
        /// </summary>
        private void MnuItemHistoricalData_Click(object? sender, EventArgs e)
        {
            using var form =
                new stockadd();

            form.ShowDialog(this);
        }


        // ── Toolbar: Run EOD Now ────────────────────────────────────────────────

        private void TsBtnRunNow_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                var exePath =
                    EodServiceLauncher.ResolveExePath();

                if (!File.Exists(exePath))
                {
                    SetStatus(
                        "✘ EODService.exe not found. " +
                        "Build the project first.",
                        success: false);

                    return;
                }


                var psi =
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = exePath,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                System.Diagnostics.Process.Start(psi);

                SetStatus(
                    "▶ EOD import started manually. " +
                    "Check the log below for progress.",
                    success: true);

                AppendLog(
                    $"[{DateTime.Now:HH:mm:ss}] " +
                    "▶ Manual EOD import triggered by user.");
            }
            catch (Exception ex)
            {
                SetStatus(
                    $"✘ Failed to start EOD service: {ex.Message}",
                    success: false);

                AppendLogError(
                    $"Manual run error: {ex.Message}");
            }
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


        // ── Cleanup ─────────────────────────────────────────────────────────────

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            _logPollTimer.Stop();
            _logPollTimer.Dispose();

            _dbContext.Dispose();

            base.OnFormClosed(e);
        }
    }
}
