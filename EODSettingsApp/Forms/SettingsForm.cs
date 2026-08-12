using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EODService.DTOs.EOD;
using EODService.Logging;
using EODService.Persistance;
using EODService.Services;
using EODSettingsApp.AppSettingsConfig;
using EODSettingsApp.ExternalConfig;
using EODSettingsApp.Services;

namespace EODSettingsApp.Forms
{
    public partial class SettingsForm : Form
    {
        private System.Windows.Forms.Timer _logPollTimer = new();
        private long _lastLogFilePosition = 0;

        public SettingsForm()
        {
            InitializeComponent();
            SetupGridColumns();

            // 1. Asynchronously load UI settings and database container records immediately on startup
            _ = LoadCurrentSettingsAsync();

            // 2. Start background log monitoring and run initial poll immediately (no 2-sec delay)
            InitializeBackgroundLogAndGridMonitoring();
        }

        // ── Startup: read settings and populate UI + DataGridView Container ───────
        private async Task LoadCurrentSettingsAsync()
        {
            try
            {
                // 1. Active provider
                var extSettings = ExternalSettingsService.Load();
                var currentProvider = extSettings.ProviderSettings.ActiveProvider;
                var index = cmbProvider.Items.IndexOf(currentProvider);
                cmbProvider.SelectedIndex = index >= 0 ? index : 0;

                // 2. Schedule settings
                var appSettings = AppSettingsService.Load();
                PopulateScheduleUI(appSettings.ScheduleSettings);

                // 3. Retrieve and populate existing database records into DataGridView container asynchronously
                await RefreshGridFromDatabaseAsync();
            }
            catch (Exception ex)
            {
                cmbProvider.SelectedIndex = 0;
                SetStatus($"Warning: Could not load full settings ({ex.Message})", success: false);
            }
        }

        private void PopulateScheduleUI(ScheduleSettingsSection schedule)
        {
            chkEnableSchedule.CheckedChanged -= ChkEnableSchedule_CheckedChanged;
            chkEnableSchedule.Checked = schedule.Enabled;
            ToggleScheduleControlsState(schedule.Enabled);

            var days = schedule.WorkingDays ?? new List<string>();
            chkMon.Checked = days.Contains("Monday", StringComparer.OrdinalIgnoreCase);
            chkTue.Checked = days.Contains("Tuesday", StringComparer.OrdinalIgnoreCase);
            chkWed.Checked = days.Contains("Wednesday", StringComparer.OrdinalIgnoreCase);
            chkThu.Checked = days.Contains("Thursday", StringComparer.OrdinalIgnoreCase);
            chkFri.Checked = days.Contains("Friday", StringComparer.OrdinalIgnoreCase);
            chkSat.Checked = days.Contains("Saturday", StringComparer.OrdinalIgnoreCase);
            chkSun.Checked = days.Contains("Sunday", StringComparer.OrdinalIgnoreCase);

            if (TimeSpan.TryParse(schedule.RunTime, out var runTime))
            {
                dtpRunTime.Value = DateTime.Today.Add(runTime);
            }
            else
            {
                dtpRunTime.Value = DateTime.Today.AddHours(18); // default 18:00
            }

            UpdateNextRunIndicator(schedule);
            chkEnableSchedule.CheckedChanged += ChkEnableSchedule_CheckedChanged;
        }

        private void ChkEnableSchedule_CheckedChanged(object? sender, EventArgs e)
        {
            bool isEnabled = chkEnableSchedule.Checked;
            ToggleScheduleControlsState(isEnabled);

            try
            {
                // Automatically save Enabled state to AppSettings.json and update Task Scheduler without manual save
                var appSettings = AppSettingsService.Load();
                appSettings.ScheduleSettings.Enabled = isEnabled;
                appSettings.ScheduleSettings.WorkingDays = CollectSelectedWorkingDays();
                appSettings.ScheduleSettings.RunTime = dtpRunTime.Value.ToString("HH:mm:ss");
                AppSettingsService.Save(appSettings);

                var exePath = EodServiceLauncher.ResolveExePath();
                WindowsTaskSchedulerService.RegisterOrUpdateTask(appSettings.ScheduleSettings, exePath);

                UpdateNextRunIndicator(appSettings.ScheduleSettings);
                SetStatus($"✔ Automated schedule {(isEnabled ? "enabled" : "disabled")}.", success: true);
                AppendLog($"[Schedule] Auto-schedule {(isEnabled ? "enabled" : "disabled")}.");
            }
            catch (Exception ex)
            {
                SetStatus($"✘ Error updating schedule: {ex.Message}", success: false);
            }
        }

        private void ToggleScheduleControlsState(bool isEnabled)
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

        // ── Define the DataGridView columns ──────────────────────────────────────
        private void SetupGridColumns()
        {
            dgvResults.Columns.Clear();
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Symbol", HeaderText = "Symbol", DataPropertyName = "Symbol" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", DataPropertyName = "Date" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Open", HeaderText = "Open", DataPropertyName = "Open" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "High", HeaderText = "High", DataPropertyName = "High" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Low", HeaderText = "Low", DataPropertyName = "Low" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Close", HeaderText = "Close", DataPropertyName = "Close" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "AdjustedClose", HeaderText = "Adj. Close", DataPropertyName = "AdjustedClose" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Volume", HeaderText = "Volume", DataPropertyName = "Volume" });
        }

        // ── "Save Schedule" Button Click ──────────────────────────────────────────
        private void BtnSaveSchedule_Click(object? sender, EventArgs e)
        {
            if (cmbProvider.SelectedItem == null)
            {
                MessageBox.Show("Please select an active provider.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProvider = cmbProvider.SelectedItem.ToString()!;
            var selectedDays = CollectSelectedWorkingDays();

            if (chkEnableSchedule.Checked && selectedDays.Count == 0)
            {
                MessageBox.Show("Please select at least one working day for the automated schedule.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. Save active provider to external config
                ExternalSettingsService.Save(new ExternalSettings
                {
                    ProviderSettings = new ProviderSettingsSection { ActiveProvider = selectedProvider }
                });

                // 2. Build schedule settings model
                var scheduleSection = new ScheduleSettingsSection
                {
                    Enabled = chkEnableSchedule.Checked,
                    WorkingDays = selectedDays,
                    RunTime = dtpRunTime.Value.ToString("HH:mm:ss")
                };

                // 3. Save to AppSettings.json
                var appSettings = AppSettingsService.Load();
                appSettings.ScheduleSettings = scheduleSection;
                AppSettingsService.Save(appSettings);

                // 4. Register or update Windows Task Scheduler
                var exePath = EodServiceLauncher.ResolveExePath();
                WindowsTaskSchedulerService.RegisterOrUpdateTask(scheduleSection, exePath);

                // 5. Update UI status & Next Run indicator
                UpdateNextRunIndicator(scheduleSection);
                SetStatus($"✔ Schedule saved & Windows Task updated (Provider: {selectedProvider}).", success: true);
                AppendLog($"[Schedule] Settings saved successfully. Windows Task '{ (chkEnableSchedule.Checked ? "Updated" : "Disabled") }'.");
            }
            catch (Exception ex)
            {
                SetStatus($"✘ Save schedule failed: {ex.Message}", success: false);
                AppendLogError($"Save schedule error: {ex.Message}");
                MessageBox.Show($"Failed to save schedule settings:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Dynamic Next Run Calculation ──────────────────────────────────────────
        private void UpdateNextRunIndicator(ScheduleSettingsSection schedule)
        {
            var nextRun = CalculateNextRunTime(schedule);
            if (nextRun.HasValue)
            {
                lblNextRunStatus.ForeColor = Color.FromArgb(30, 58, 138);
                lblNextRunStatus.Text = $"🕒 Next Scheduled Run: {nextRun.Value:dddd, MMM d, yyyy 'at' HH:mm}";
            }
            else
            {
                lblNextRunStatus.ForeColor = Color.FromArgb(185, 28, 28);
                lblNextRunStatus.Text = "🕒 Next Scheduled Run: Automated schedule is disabled or has no working days selected.";
            }
        }

        private DateTime? CalculateNextRunTime(ScheduleSettingsSection settings)
        {
            if (!settings.Enabled || settings.WorkingDays == null || !settings.WorkingDays.Any())
                return null;

            if (!TimeSpan.TryParse(settings.RunTime, out var runTime))
                return null;

            var now = DateTime.Now;
            for (int i = 0; i < 7; i++)
            {
                var candidateDate = now.Date.AddDays(i);
                var candidateDateTime = candidateDate.Add(runTime);

                if (settings.WorkingDays.Contains(candidateDate.DayOfWeek.ToString(), StringComparer.OrdinalIgnoreCase))
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
            var days = new List<string>();
            if (chkMon.Checked) days.Add("Monday");
            if (chkTue.Checked) days.Add("Tuesday");
            if (chkWed.Checked) days.Add("Wednesday");
            if (chkThu.Checked) days.Add("Thursday");
            if (chkFri.Checked) days.Add("Friday");
            if (chkSat.Checked) days.Add("Saturday");
            if (chkSun.Checked) days.Add("Sunday");
            return days;
        }

        // ── Background Log Monitoring & Grid Auto Refresh ────────────────────────
        private void InitializeBackgroundLogAndGridMonitoring()
        {
            var logPath = FileLoggerProvider.LogFilePath;
            if (File.Exists(logPath))
            {
                _lastLogFilePosition = new FileInfo(logPath).Length;
            }

            _logPollTimer.Interval = 2000; // Poll every 2 seconds
            _logPollTimer.Tick += async (s, e) => await PollLogFileAndRefreshGridAsync();
            _logPollTimer.Start();

            // Run initial log poll immediately on launch without waiting 2000ms
            _ = PollLogFileAndRefreshGridAsync();
        }

        private async Task PollLogFileAndRefreshGridAsync()
        {
            var logPath = FileLoggerProvider.LogFilePath;
            if (!File.Exists(logPath)) return;

            try
            {
                using var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (fs.Length < _lastLogFilePosition)
                {
                    _lastLogFilePosition = 0; // File was truncated/re-created
                }

                if (fs.Length > _lastLogFilePosition)
                {
                    fs.Seek(_lastLogFilePosition, SeekOrigin.Begin);
                    using var reader = new StreamReader(fs);
                    string? newContent = await reader.ReadToEndAsync();
                    _lastLogFilePosition = fs.Position;

                    if (!string.IsNullOrWhiteSpace(newContent))
                    {
                        AppendLog(newContent.TrimEnd());

                        // If background service completed execution, auto-update grid and advance Next Run Indicator
                        if (newContent.Contains("completed successfully") || newContent.Contains("EOD import complete"))
                        {
                            await RefreshGridFromDatabaseAsync();
                            var updatedSettings = AppSettingsService.Load();
                            UpdateNextRunIndicator(updatedSettings.ScheduleSettings);
                        }
                    }
                }
            }
            catch
            {
                // Best effort log reading
            }
        }

        private async Task RefreshGridFromDatabaseAsync()
        {
            try
            {
                var appSettingsPath = AppSettingsPath.Resolve();
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Path.GetDirectoryName(appSettingsPath)!)
                    .AddJsonFile(Path.GetFileName(appSettingsPath), optional: true, reloadOnChange: false)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_DB_USER"))
                    return;

                using var dbContext = AppDbContextFactory.Create(connectionString);
                var dailyRecords = await dbContext.EodDaily.AsNoTracking().ToListAsync();

                if (dailyRecords != null && dailyRecords.Any())
                {
                    // Thread-safe update onto WinForms UI thread
                    Invoke(() =>
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
                        lblGridTitle.Text = $"EOD Results (Automated Service Operations) — {dailyRecords.Count} record(s)";
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[SettingsForm] RefreshGridFromDatabaseAsync error: {ex.Message}");
            }
        }

        // ── DataGrid & Logging Helpers ───────────────────────────────────────────
        public void PopulateGrid(IEnumerable<EodData> results)
        {
            dgvResults.Rows.Clear();
            foreach (var r in results)
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
            lblGridTitle.Text = $"EOD Results  ({results.Count()} symbols)";
        }

        private void AppendLog(string message)
        {
            if (rtbLogs.IsDisposed) return;
            Invoke(() =>
            {
                rtbLogs.AppendText($"{message}{Environment.NewLine}");
                rtbLogs.SelectionStart = rtbLogs.Text.Length;
                rtbLogs.ScrollToCaret();
            });
        }

        private void AppendLogError(string message)
        {
            if (rtbLogs.IsDisposed) return;
            Invoke(() =>
            {
                rtbLogs.SelectionColor = Color.Red;
                rtbLogs.AppendText($"ERROR: {message}{Environment.NewLine}");
                rtbLogs.SelectionColor = rtbLogs.ForeColor;
                rtbLogs.SelectionStart = rtbLogs.Text.Length;
                rtbLogs.ScrollToCaret();
            });
        }

        private void SetStatus(string message, bool success)
        {
            lblStatus.ForeColor = success
                ? Color.FromArgb(22, 163, 74)   // green
                : Color.FromArgb(185, 28, 28);  // red
            lblStatus.Text = message;
        }

        // ── Menu handlers ────────────────────────────────────────────────────────
        private void MnuItemProviderSettings_Click(object? sender, EventArgs e)
        {
            using var form = new ProviderSettingsForm();
            form.ShowDialog(this);
        }

        private void MnuItemSymbolSettings_Click(object? sender, EventArgs e)
        {
            using var form = new SymbolSettingsForm();
            form.ShowDialog(this);
        }
    }
}
