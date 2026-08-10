using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

using EODService.DTOs.ProviderSettings;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.TwelveDataSettings;
using EODService.DTOs.YahooSettings;
using EODService.DTOs.EOD;
using EODService.Services;
using EODService.Persistance;

using EODSettingsApp.ExternalConfig;
using EODSettingsApp.Services;

namespace EODSettingsApp.Forms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            LoadCurrentSettings();
            SetupGridColumns();
        }

        // ── Startup: read external config and pre-select the last used provider ──
        private void LoadCurrentSettings()
        {
            try
            {
                var settings = ExternalSettingsService.Load();
                var currentProvider = settings.ProviderSettings.ActiveProvider;
                var index = cmbProvider.Items.IndexOf(currentProvider);
                cmbProvider.SelectedIndex = index >= 0 ? index : 0;
            }
            catch
            {
                cmbProvider.SelectedIndex = 0;
            }
        }

        // ── Define the DataGridView columns once ─────────────────────────────────
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

        // ── "Get Data" button ────────────────────────────────────────────────────
        private async void BtnGetData_Click(object? sender, EventArgs e)
        {
            if (cmbProvider.SelectedItem == null)
            {
                MessageBox.Show("Please select a provider.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProvider = cmbProvider.SelectedItem.ToString()!;

            // 1. Save the chosen provider to the external config
            ExternalSettingsService.Save(new ExternalSettings
            {
                ProviderSettings = new ProviderSettingsSection { ActiveProvider = selectedProvider }
            });

            // 2. Lock the UI while running
            SetUiBusy(true, $"Fetching data from {selectedProvider}...");
            dgvResults.Rows.Clear();

            try
            {
                // 3. Load all settings (external file overrides appsettings.json for ActiveProvider)
                var providerSettings = ProviderSettingsMapper.MapToProviderSettings();
                var symbolSettings = SymbolSettingsMapper.MapToSymbolSettings();
                var yahooSettings = YahooSettingsMapper.MapToYahooSettings();
                var twelveDataSettings = TwelveDataSettingsMapper.MapToTwelveDataSettings();

                if (providerSettings == null || symbolSettings == null)
                    throw new Exception("Could not load settings from appsettings.json.");

                // 4. Build the UI Logger
                var uiLogger = new Logging.UiLoggerProvider();
                uiLogger.OnLog = (msg) =>
                {
                    if (IsDisposed) return;
                    // Safely update the RichTextBox from background threads
                    Invoke(() =>
                    {
                        rtbLogs.AppendText(msg + Environment.NewLine);
                        rtbLogs.SelectionStart = rtbLogs.Text.Length;
                        rtbLogs.ScrollToCaret();
                    });
                };

                // 5. Build the HTTP client and service
                using var loggerFactory = LoggerFactory.Create(b => 
                {
                    b.AddProvider(uiLogger);
                });
                
                using var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var service = EODServiceFactory.CreateProvider(
                    providerName: providerSettings.ActiveProvider,
                    symbolSettings: symbolSettings,
                    httpClient: httpClient,
                    loggerFactory: loggerFactory,
                    yahooSettings: yahooSettings,
                    twelveDataSettings: twelveDataSettings);

                // 6. Fetch data
                uiLogger.OnLog("Starting EOD data import...");
                SetUiBusy(true, "Calling API...");
                var results = await service.GetEodDataAsync();

                if (results == null || !results.Any())
                {
                    uiLogger.OnLog("No data returned from the API.");
                    SetStatus("No data returned from the API.", success: false);
                    return;
                }

                // 7. Display results in the grid
                uiLogger.OnLog($"Successfully downloaded {results.Count()} records.");
                PopulateGrid(results);
                SetStatus($"{results.Count()} record(s) fetched. Saving to database...", success: true);

                // 8. Save to Oracle DB
                uiLogger.OnLog("Saving to Oracle Database...");
                await SaveToDatabase(results);

                uiLogger.OnLog("Data saved to Oracle database successfully.");
                SetStatus($"✔  {results.Count()} record(s) fetched and saved to Oracle DB successfully.", success: true);
            }
            catch (Exception ex)
            {
                SetStatus($"✘  Error: {ex.Message}", success: false);
                Invoke(() =>
                {
                    rtbLogs.SelectionColor = Color.Red;
                    rtbLogs.AppendText($"ERROR: {ex.Message}{Environment.NewLine}");
                    rtbLogs.SelectionColor = rtbLogs.ForeColor;
                });
                MessageBox.Show($"An error occurred:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUiBusy(false, "");
            }
        }

        // ── Populate the DataGridView with fetched EodData ───────────────────────
        private void PopulateGrid(IEnumerable<EodData> results)
        {
            dgvResults.Rows.Clear();
            foreach (var r in results)
            {
                dgvResults.Rows.Add(
                    r.Symbol,
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

        // ── Save the fetched data to Oracle ──────────────────────────────────────
        private async Task SaveToDatabase(IEnumerable<EodData> results)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(EODService.Config.PathesConfig.AppSettingsFileName, optional: false, reloadOnChange: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"Connection string 'DefaultConnection' is missing in {EODService.Config.PathesConfig.AppSettingsFileName}.");

            using var dbContext = AppDbContextFactory.Create(connectionString);
            await dbContext.Database.EnsureCreatedAsync();

            await EodPersistenceService.SaveEodDataAsync(results, dbContext);
        }

        // ── UI helpers ───────────────────────────────────────────────────────────
        private void SetUiBusy(bool busy, string message)
        {
            btnGetData.Enabled = !busy;
            cmbProvider.Enabled = !busy;
            btnGetData.Text = busy ? "Running..." : "Get Data";
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
            if (!string.IsNullOrEmpty(message))
                SetStatus(message, success: true);
        }

        private void SetStatus(string message, bool success)
        {
            lblStatus.ForeColor = success
                ? Color.FromArgb(22, 163, 74)   // green
                : Color.FromArgb(185, 28, 28);  // red
            lblStatus.Text = message;
        }

        // ── Menu handlers ────────────────────────────────────────────────────────

        /// <summary>
        /// Settings → Provider Settings: opens the provider API settings dialog.
        /// </summary>
        private void MnuItemProviderSettings_Click(object? sender, EventArgs e)
        {
            using var form = new ProviderSettingsForm();
            form.ShowDialog(this);
        }
    }
}
