using System;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EODService.Config;
using EODService.Models.Provider;
using EODService.Persistance;
using EODService.Persistance.Repo;
using EODSettingsApp.AppSettingsConfig;
using EODService.Models;

namespace EODSettingsApp.Forms
{
    /// <summary>
    /// Modal dialog for viewing and editing Yahoo and TwelveData provider
    /// settings and JSON parameters stored in Oracle Database (table PROVIDER).
    /// </summary>
    public partial class ProviderSettingsForm : Form
    {
        public ProviderSettingsForm()
        {
            InitializeComponent();
            AppIconHelper.ApplyAppIconAndTitle(this);
            Load += async (_, _) => await LoadProviderSettingsAsync();
        }

        public ProviderSettingsForm(int activeTab) : this()
        {
            // Hide the tab headers to make it look like a unified form
            tabProviders.Appearance = TabAppearance.FlatButtons;
            tabProviders.ItemSize = new Size(0, 1);
            tabProviders.SizeMode = TabSizeMode.Fixed;

            if (activeTab == 0)
            {
                tabProviders.SelectedIndex = 0;
                lblTitle.Text = "Yahoo Finance Settings";
                lblSubtitle.Text = "Configure Yahoo Finance Base URL, Endpoint, and JSON parameters";
            }
            else if (activeTab == 1)
            {
                tabProviders.SelectedIndex = 1;
                lblTitle.Text = "TwelveData Settings";
                lblSubtitle.Text = "Configure TwelveData Base URL, Endpoint, API Key, and JSON parameters";
            }
            else if (activeTab == 2)
            {
                tabProviders.SelectedIndex = 2;
                lblTitle.Text = "Reuters (LSEG) Settings";
                lblSubtitle.Text = "Configure Reuters WebSocket Base URL, Endpoint, and JSON parameters";
            }
        }

        // ── Load ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Loads provider records from the Oracle database PROVIDER table and populates fields.
        /// </summary>
        private async Task LoadProviderSettingsAsync()
        {
            try
            {
                SetStatus(success: true, "Loading provider configurations from Oracle Database...");

                var connectionString = ConnectionStringResolver.Get();
                if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_DB_USER"))
                {
                    LoadFromAppSettingsFallback("Database connection string is missing or invalid. Loaded defaults.");
                    return;
                }

                using var dbContext = AppDbContextFactory.Create(connectionString);
                var providerRepo = new ProviderRepo(dbContext);
                var providers = await providerRepo.GetAllProvidersAsync();

                if (IsDisposed) return;

                if (providers == null || !providers.Any())
                {
                    LoadFromAppSettingsFallback("No providers found in database table PROVIDER. Loaded fallback defaults.");
                    return;
                }

                var yahooProvider = providers.FirstOrDefault(p => p.Id == (int)ProviderIds.Yahoo || p.Name.Contains("Yahoo", StringComparison.OrdinalIgnoreCase));
                if (yahooProvider != null)
                {
                    txtYahooBaseUrl.Text    = yahooProvider.BaseUrl ?? "https://query1.finance.yahoo.com";
                    txtYahooEndpoint.Text   = yahooProvider.EndPoint ?? "/v8/finance/chart/";
                    txtYahooApiKey.Text     = yahooProvider.ApiKey ?? string.Empty;
                    txtYahooParameters.Text = FormatJsonString(yahooProvider.Parameters ?? JsonSerializer.Serialize(new
                    {
                        interval = "1d",
                        range = "1d"
                    }));
                }
                else
                {
                    txtYahooBaseUrl.Text    = "https://query1.finance.yahoo.com";
                    txtYahooEndpoint.Text   = "/v8/finance/chart/";
                    txtYahooApiKey.Text     = string.Empty;
                    txtYahooParameters.Text = FormatJsonString(JsonSerializer.Serialize(new
                    {
                        interval = "1d",
                        range = "1d"
                    }));
                }

                var twelveProvider = providers.FirstOrDefault(p => p.Id == (int)ProviderIds.TwelveData || p.Name.Contains("Twelve", StringComparison.OrdinalIgnoreCase));
                if (twelveProvider != null)
                {
                    txtTwelveBaseUrl.Text    = twelveProvider.BaseUrl ?? "https://api.twelvedata.com";
                    txtTwelveEndpoint.Text   = twelveProvider.EndPoint ?? "/time_series";
                    txtTwelveApiKey.Text     = twelveProvider.ApiKey ?? string.Empty;
                    txtTwelveParameters.Text = FormatJsonString(twelveProvider.Parameters ?? JsonSerializer.Serialize(new
                    {
                        interval = "1day",
                        outputsize = "1"
                    }));
                }
                else
                {
                    txtTwelveBaseUrl.Text    = "https://api.twelvedata.com";
                    txtTwelveEndpoint.Text   = "/time_series";
                    txtTwelveApiKey.Text     = string.Empty;
                    txtTwelveParameters.Text = FormatJsonString(JsonSerializer.Serialize(new
                    {
                        interval = "1day",
                        outputsize = "1"
                    }));
                }

                var reutersProvider = providers.FirstOrDefault(p => p.Id == (int)ProviderIds.Reuters || p.Name.Contains("Reuter", StringComparison.OrdinalIgnoreCase));
                if (reutersProvider != null)
                {
                    txtReutersBaseUrl.Text    = reutersProvider.BaseUrl ?? "ws://10.110.221.99:15000";
                    txtReutersEndpoint.Text   = reutersProvider.EndPoint ?? "/WebSocket";
                    txtReutersApiKey.Text     = reutersProvider.ApiKey ?? string.Empty;
                    txtReutersParameters.Text = FormatJsonString(reutersProvider.Parameters ?? JsonSerializer.Serialize(new
                    {
                        DacsUser = "EODService",
                        ApplicationId = "256",
                        ServiceName = "ELEKTRON_DD"
                    }));
                }
                else
                {
                    txtReutersBaseUrl.Text    = "ws://10.110.221.99:15000";
                    txtReutersEndpoint.Text   = "/WebSocket";
                    txtReutersApiKey.Text     = string.Empty;
                    txtReutersParameters.Text = FormatJsonString(JsonSerializer.Serialize(new
                    {
                        DacsUser = "EODService",
                        ApplicationId = "256",
                        ServiceName = "ELEKTRON_DD"
                    }));
                }

                SetStatus(success: true, "✔ Loaded provider settings from database (PROVIDER).");
            }
            catch (Exception ex)
            {
                LoadFromAppSettingsFallback($"✘ Could not load from database ({ex.Message}). Loaded fallback settings.");
            }
        }

        private void LoadFromAppSettingsFallback(string statusMessage)
        {
            try
            {
                var model = AppSettingsService.Load();

                txtYahooBaseUrl.Text    = model.YahooSettings.BaseUrl;
                txtYahooEndpoint.Text   = model.YahooSettings.Endpoint;
                txtYahooApiKey.Text     = string.Empty;
                txtYahooParameters.Text = FormatJsonString(JsonSerializer.Serialize(new
                {
                    interval = model.YahooSettings.Interval,
                    range = model.YahooSettings.Range
                }));

                txtTwelveBaseUrl.Text    = model.TwelveDataSettings.BaseUrl;
                txtTwelveEndpoint.Text   = model.TwelveDataSettings.Endpoint;
                txtTwelveApiKey.Text     = model.TwelveDataSettings.ApiKey;
                txtTwelveParameters.Text = FormatJsonString(JsonSerializer.Serialize(new
                {
                    interval = model.TwelveDataSettings.Interval,
                    outputsize = model.TwelveDataSettings.OutputSize
                }));

                txtReutersBaseUrl.Text    = "ws://10.110.221.99:15000";
                txtReutersEndpoint.Text   = "/WebSocket";
                txtReutersApiKey.Text     = string.Empty;
                txtReutersParameters.Text = FormatJsonString(JsonSerializer.Serialize(new
                {
                    DacsUser = "EODService",
                    ApplicationId = "256",
                    ServiceName = "ELEKTRON_DD"
                }));

                SetStatus(success: false, statusMessage);
            }
            catch (Exception fallbackEx)
            {
                SetStatus(success: false, $"✘ Failed loading provider settings: {fallbackEx.Message}");
            }
        }

        // ── Save ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Validates JSON in Parameters fields, then saves provider configurations to Oracle Database (PROVIDER table).
        /// </summary>
        private async void BtnSaveProviderSettings_Click(object? sender, EventArgs e)
        {
            // 1. Validate Yahoo Parameters JSON
            if (!TryValidateJson(txtYahooParameters.Text, out var yahooNormalizedJson, out var yahooError))
            {
                SetStatus(success: false, $"✘ Yahoo JSON Error: {yahooError}");
                tabProviders.SelectedIndex = 0;
                txtYahooParameters.Focus();
                MessageBox.Show(
                    $"Yahoo Parameters contains invalid JSON:\n\n{yahooError}\n\nPlease fix the JSON format before saving.",
                    "JSON Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 2. Validate TwelveData Parameters JSON
            if (!TryValidateJson(txtTwelveParameters.Text, out var twelveNormalizedJson, out var twelveError))
            {
                SetStatus(success: false, $"✘ TwelveData JSON Error: {twelveError}");
                tabProviders.SelectedIndex = 1;
                txtTwelveParameters.Focus();
                MessageBox.Show(
                    $"TwelveData Parameters contains invalid JSON:\n\n{twelveError}\n\nPlease fix the JSON format before saving.",
                    "JSON Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 3. Validate Reuters Parameters JSON
            if (!TryValidateJson(txtReutersParameters.Text, out var reutersNormalizedJson, out var reutersError))
            {
                SetStatus(success: false, $"✘ Reuters JSON Error: {reutersError}");
                tabProviders.SelectedIndex = 2;
                txtReutersParameters.Focus();
                MessageBox.Show(
                    $"Reuters Parameters contains invalid JSON:\n\n{reutersError}\n\nPlease fix the JSON format before saving.",
                    "JSON Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 4. Validate Base URLs format
            if (!string.IsNullOrWhiteSpace(txtYahooBaseUrl.Text) && !Uri.TryCreate(txtYahooBaseUrl.Text.Trim(), UriKind.Absolute, out _))
            {
                SetStatus(success: false, "✘ Yahoo Base URL is not a valid absolute URL (e.g. https://query1.finance.yahoo.com).");
                tabProviders.SelectedIndex = 0;
                txtYahooBaseUrl.Focus();
                MessageBox.Show("Yahoo Base URL must be a valid absolute URL.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtTwelveBaseUrl.Text) && !Uri.TryCreate(txtTwelveBaseUrl.Text.Trim(), UriKind.Absolute, out _))
            {
                SetStatus(success: false, "✘ TwelveData Base URL is not a valid absolute URL (e.g. https://api.twelvedata.com).");
                tabProviders.SelectedIndex = 1;
                txtTwelveBaseUrl.Focus();
                MessageBox.Show("TwelveData Base URL must be a valid absolute URL.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtReutersBaseUrl.Text) && !Uri.TryCreate(txtReutersBaseUrl.Text.Trim(), UriKind.Absolute, out _))
            {
                SetStatus(success: false, "✘ Reuters Base URL is not a valid WebSocket URL (e.g. ws://10.110.221.99:15000).");
                tabProviders.SelectedIndex = 2;
                txtReutersBaseUrl.Focus();
                MessageBox.Show("Reuters Base URL must be a valid WebSocket URL.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnSaveProviderSettings.Enabled = false;
                SetStatus(success: true, "Saving provider changes to Oracle Database (PROVIDER)...");

                var connectionString = ConnectionStringResolver.Get();
                using var dbContext = AppDbContextFactory.Create(connectionString);
                var providerRepo = new ProviderRepo(dbContext);

                // Update Yahoo Finance (ID = 1)
                await providerRepo.UpdateProvider(
                    providerId: 1,
                    name: "Yahoo",
                    baseUrl: txtYahooBaseUrl.Text.Trim(),
                    endPoint: txtYahooEndpoint.Text.Trim(),
                    apiKey: string.IsNullOrWhiteSpace(txtYahooApiKey.Text) ? null : txtYahooApiKey.Text.Trim(),
                    parameters: yahooNormalizedJson);

                // Update TwelveData (ID = 2)
                await providerRepo.UpdateProvider(
                    providerId: 2,
                    name: "TwelveData",
                    baseUrl: txtTwelveBaseUrl.Text.Trim(),
                    endPoint: txtTwelveEndpoint.Text.Trim(),
                    apiKey: string.IsNullOrWhiteSpace(txtTwelveApiKey.Text) ? null : txtTwelveApiKey.Text.Trim(),
                    parameters: twelveNormalizedJson);

                // Update Reuters (ID = 3)
                await providerRepo.UpdateProvider(
                    providerId: 3,
                    name: "Reuters",
                    baseUrl: txtReutersBaseUrl.Text.Trim(),
                    endPoint: txtReutersEndpoint.Text.Trim(),
                    apiKey: string.IsNullOrWhiteSpace(txtReutersApiKey.Text) ? null : txtReutersApiKey.Text.Trim(),
                    parameters: reutersNormalizedJson);

                // Update AppSettings.json for fallback synchronization
                try
                {
                    var currentModel = AppSettingsService.Load();
                    currentModel.YahooSettings.BaseUrl    = txtYahooBaseUrl.Text.Trim();
                    currentModel.YahooSettings.Endpoint   = txtYahooEndpoint.Text.Trim();
                    currentModel.TwelveDataSettings.BaseUrl    = txtTwelveBaseUrl.Text.Trim();
                    currentModel.TwelveDataSettings.Endpoint   = txtTwelveEndpoint.Text.Trim();
                    currentModel.TwelveDataSettings.ApiKey     = txtTwelveApiKey.Text.Trim();
                    AppSettingsService.Save(currentModel);
                }
                catch
                {
                    // Ignore AppSettings write failure if DB succeeded
                }

                SetStatus(success: true, "✔ Provider settings saved successfully to Oracle DB (PROVIDER).");
                MessageBox.Show(
                    "Provider settings and JSON parameters saved successfully to Oracle Database!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Save failed: {ex.Message}");
                MessageBox.Show(
                    $"Failed to save provider settings to database:\n\n{ex.Message}",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnSaveProviderSettings.Enabled = true;
            }
        }

        // ── JSON Utilities ────────────────────────────────────────────────────────

        /// <summary>
        /// Validates that a string is a valid JSON document (specifically a JSON Object).
        /// </summary>
        private static bool TryValidateJson(string? json, out string normalizedJson, out string errorMessage)
        {
            normalizedJson = string.Empty;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(json))
            {
                errorMessage = "Parameters cannot be empty. Must be a valid JSON object (e.g. {}).";
                return false;
            }

            try
            {
                using var doc = JsonDocument.Parse(json.Trim());
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    errorMessage = "Parameters must be a JSON object (enclosed in { ... }).";
                    return false;
                }

                // Minify or produce valid JSON string for storage
                normalizedJson = JsonSerializer.Serialize(doc.RootElement);
                return true;
            }
            catch (JsonException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Formats a JSON string with 2-space indentation for easy visual editing.
        /// </summary>
        private static string FormatJsonString(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "{\r\n}";

            try
            {
                using var doc = JsonDocument.Parse(json.Trim());
                return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch
            {
                return json;
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private void SetStatus(bool success, string message)
        {
            lblProviderSettingsStatus.ForeColor = success
                ? Color.FromArgb(22, 163, 74)
                : Color.FromArgb(185, 28, 28);
            lblProviderSettingsStatus.Text = message;
        }
    }
}
