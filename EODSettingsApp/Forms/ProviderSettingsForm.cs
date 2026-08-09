using System;
using System.Windows.Forms;
using EODSettingsApp.AppSettingsConfig;

namespace EODSettingsApp.Forms
{
    /// <summary>
    /// Modal dialog for viewing and editing the Yahoo and TwelveData provider
    /// API settings stored in EODService's AppSettings.json.
    /// </summary>
    public partial class ProviderSettingsForm : Form
    {
        public ProviderSettingsForm()
        {
            InitializeComponent();
            LoadProviderSettings();
        }

        // ── Load ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads AppSettings.json and populates all text boxes on startup.
        /// </summary>
        private void LoadProviderSettings()
        {
            try
            {
                var model = AppSettingsService.Load();
                PopulateYahooFields(model.YahooSettings);
                PopulateTwelveDataFields(model.TwelveDataSettings);
            }
            catch (Exception ex)
            {
                SetStatus(success: false,
                    $"✘  Could not load AppSettings: {ex.Message}");
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
        /// Validates fields, builds the model, and writes it to AppSettings.json.
        /// </summary>
        private void BtnSaveProviderSettings_Click(object? sender, EventArgs e)
        {
            if (!TryCollectAndSaveProviderSettings())
                return;

            SetStatus(success: true, "✔  Provider settings saved successfully.");
        }

        private bool TryCollectAndSaveProviderSettings()
        {
            if (!TryParseOutputSize(txtTwelveOutputSize.Text, out var outputSize))
            {
                SetStatus(success: false, "✘  Output Size must be a positive integer.");
                return false;
            }

            try
            {
                AppSettingsService.Save(new AppSettingsModel
                {
                    YahooSettings      = CollectYahooSettings(),
                    TwelveDataSettings = CollectTwelveDataSettings(outputSize)
                });

                return true;
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘  Save failed: {ex.Message}");
                return false;
            }
        }

        private YahooSettingsSection CollectYahooSettings() => new()
        {
            BaseUrl  = txtYahooBaseUrl.Text.Trim(),
            Endpoint = txtYahooEndpoint.Text.Trim(),
            Interval = txtYahooInterval.Text.Trim(),
            Range    = txtYahooRange.Text.Trim()
        };

        private TwelveDataSettingsSection CollectTwelveDataSettings(int outputSize) => new()
        {
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
    }
}
