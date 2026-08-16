using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using EODService.DTOs.OracleSettings;
using EODService.Persistance;
using EODService.Persistance.Repo;
using EODService.Services;
using EODSettingsApp.AppSettingsConfig;

namespace EODSettingsApp.Forms
{
    /// <summary>
    /// Modal dialog for viewing and editing the Yahoo and TwelveData provider
    /// API settings stored in Oracle DB (PROVIDER table) and EODService's AppSettings.json.
    /// </summary>
    public partial class ProviderSettingsForm : Form
    {
        public ProviderSettingsForm()
        {
            InitializeComponent();
            _ = LoadProviderSettingsAsync();
        }

        // ── Load ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads AppSettings.json and Oracle PROVIDER table to populate all text boxes on startup.
        /// </summary>
        private async Task LoadProviderSettingsAsync()
        {
            try
            {
                var model = AppSettingsService.Load();
                PopulateYahooFields(model.YahooSettings);
                PopulateTwelveDataFields(model.TwelveDataSettings);

                // Fetch BaseUrl, Endpoint, and ApiKey directly from Oracle Database PROVIDER table
                var connectionString = OracleSettingsMapper.GetConnectionString();
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    using var dbContext = AppDbContextFactory.Create(connectionString);
                    IProvider repo = new ProviderRepo(dbContext);

                    var yahooId = model.YahooSettings?.ID > 0 ? model.YahooSettings.ID : 1;
                    var twelveDataId = model.TwelveDataSettings?.ID > 0 ? model.TwelveDataSettings.ID : 2;

                    var yahooDb = await repo.GetProviderById(yahooId);
                    if (yahooDb != null)
                    {
                        txtYahooBaseUrl.Text = yahooDb.BaseUrl;
                        txtYahooEndpoint.Text = yahooDb.EndPoint;
                    }

                    var twelveDb = await repo.GetProviderById(twelveDataId);
                    if (twelveDb != null)
                    {
                        txtTwelveBaseUrl.Text  = twelveDb.BaseUrl;
                        txtTwelveEndpoint.Text = twelveDb.EndPoint;
                        txtTwelveApiKey.Text   = twelveDb.ApiKey ?? string.Empty;

                        bool isAesEncrypted = AesEncryptionService.IsAesEncrypted(twelveDb.ApiKey);
                        if (isAesEncrypted)
                            SetStatus(success: true, "✔ Provider settings loaded. 🔒 API Key stored securely (AES-256).");
                        else
                            SetStatus(success: true, "✔ Provider settings loaded from Oracle DB.");
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Could not load provider settings: {ex.Message}");
            }
        }

        private void PopulateYahooFields(YahooSettingsSection yahoo)
        {
            txtYahooBaseUrl.Text  = yahoo.BaseUrl;
            txtYahooEndpoint.Text = yahoo.Endpoint;
            txtYahooInterval.Text = yahoo.Interval;
            txtYahooRange.Text    = yahoo.Range;
        }

        private void PopulateTwelveDataFields(TwelveDataSettingsSection twelveData)
        {
            txtTwelveBaseUrl.Text    = twelveData.BaseUrl;
            txtTwelveEndpoint.Text   = twelveData.Endpoint;
            txtTwelveInterval.Text   = twelveData.Interval;
            txtTwelveOutputSize.Text = twelveData.OutputSize.ToString();
            txtTwelveApiKey.Text     = twelveData.ApiKey;
        }

        // ── Save ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Validates fields, updates AppSettings.json, and saves BaseUrl, Endpoint, and ApiKey into Oracle PROVIDER table.
        /// </summary>
        private async void BtnSaveProviderSettings_Click(object? sender, EventArgs e)
        {
            btnSaveProviderSettings.Enabled = false;
            try
            {
                if (await TryCollectAndSaveProviderSettingsAsync())
                {
                    SetStatus(success: true, "✔ Provider settings saved and API Key encrypted (AES-256). 🔒");
                }
            }
            finally
            {
                btnSaveProviderSettings.Enabled = true;
            }
        }

        private async Task<bool> TryCollectAndSaveProviderSettingsAsync()
        {
            if (!TryParseOutputSize(txtTwelveOutputSize.Text, out var outputSize))
            {
                SetStatus(success: false, "✘ Output Size must be a positive integer.");
                return false;
            }

            try
            {
                // 1. Load current model and save to AppSettings.json
                var model = AppSettingsService.Load();
                var yahooId = model.YahooSettings?.ID > 0 ? model.YahooSettings.ID : 1;
                var twelveDataId = model.TwelveDataSettings?.ID > 0 ? model.TwelveDataSettings.ID : 2;

                model.YahooSettings      = CollectYahooSettings(yahooId);
                model.TwelveDataSettings = CollectTwelveDataSettings(twelveDataId, outputSize);
                AppSettingsService.Save(model);

                // 2. Save directly to Oracle Database PROVIDER table
                var connectionString = OracleSettingsMapper.GetConnectionString();
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    using var dbContext = AppDbContextFactory.Create(connectionString);
                    IProvider repo = new ProviderRepo(dbContext);

                    await repo.UpdateProvider(
                        providerId: yahooId,
                        name: "Yahoo",
                        baseUrl: txtYahooBaseUrl.Text.Trim(),
                        endPoint: txtYahooEndpoint.Text.Trim(),
                        apiKey: null);

                    await repo.UpdateProvider(
                        providerId: twelveDataId,
                        name: "TwelveData",
                        baseUrl: txtTwelveBaseUrl.Text.Trim(),
                        endPoint: txtTwelveEndpoint.Text.Trim(),
                        apiKey: txtTwelveApiKey.Text.Trim());
                }

                return true;
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Save failed: {ex.Message}");
                return false;
            }
        }

        private YahooSettingsSection CollectYahooSettings(int id) => new()
        {
            ID       = id,
            BaseUrl  = txtYahooBaseUrl.Text.Trim(),
            Endpoint = txtYahooEndpoint.Text.Trim(),
            Interval = txtYahooInterval.Text.Trim(),
            Range    = txtYahooRange.Text.Trim()
        };

        private TwelveDataSettingsSection CollectTwelveDataSettings(int id, int outputSize) => new()
        {
            ID         = id,
            BaseUrl    = txtTwelveBaseUrl.Text.Trim(),
            Endpoint   = txtTwelveEndpoint.Text.Trim(),
            Interval   = txtTwelveInterval.Text.Trim(),
            OutputSize = outputSize,
            ApiKey     = txtTwelveApiKey.Text.Trim()
        };

        private static bool TryParseOutputSize(string text, out int value)
            => int.TryParse(text.Trim(), out value) && value > 0;

        private void SetStatus(bool success, string message)
        {
            lblProviderSettingsStatus.ForeColor = success
                ? System.Drawing.Color.FromArgb(22, 163, 74)
                : System.Drawing.Color.FromArgb(185, 28, 28);
            lblProviderSettingsStatus.Text = message;
        }

        private void ChkShowTwelveApiKey_CheckedChanged(object? sender, EventArgs e)
        {
            txtTwelveApiKey.UseSystemPasswordChar = !chkShowTwelveApiKey.Checked;
        }
    }
}

