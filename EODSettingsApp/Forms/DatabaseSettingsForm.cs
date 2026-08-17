using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using EODSettingsApp.AppSettingsConfig;
using EODService.Services;

namespace EODSettingsApp.Forms
{
    /// <summary>
    /// Modal dialog for configuring Oracle DB connection parameters, building connection strings,
    /// and testing DB connectivity in real-time.
    /// </summary>
    public partial class DatabaseSettingsForm : Form
    {
        private bool _isUpdatingFields = false;

        public DatabaseSettingsForm()
        {
            InitializeComponent();
            LoadDatabaseSettings();
        }

        // ── Load & Parse Connection Parameters ───────────────────────────────────

        private void LoadDatabaseSettings()
        {
            try
            {
                var model = AppSettingsService.Load();
                var connectionString = model.ConnectionStrings?.DefaultConnection ?? string.Empty;

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    // No saved connection yet — leave fields empty for fresh entry
                    _isUpdatingFields = true;
                    txtHost.Text        = string.Empty;
                    txtPort.Text        = string.Empty;
                    txtServiceName.Text = string.Empty;
                    txtUserId.Text      = string.Empty;
                    txtPassword.Text    = string.Empty;
                    txtConnectionString.Text = string.Empty;
                    _isUpdatingFields = false;

                    SetStatus(success: true, "ℹ No saved connection string found. Please enter database details.");
                }
                else
                {
                    txtConnectionString.Text = connectionString;
                    ParseAndPopulateFields(connectionString);

                    SetStatus(success: true, "✔ Connection settings loaded.");
                }
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Could not load connection settings: {ex.Message}");
            }
        }

        private void ParseAndPopulateFields(string connectionString)
        {
            _isUpdatingFields = true;
            try
            {
                txtHost.Text        = ExtractValue(connectionString, @"HOST\s*=\s*([^)\s;]+)");
                txtPort.Text        = ExtractValue(connectionString, @"PORT\s*=\s*([^)\s;]+)");
                txtServiceName.Text = ExtractValue(connectionString, @"SERVICE_NAME\s*=\s*([^)\s;]+)");
                txtUserId.Text      = ExtractValue(connectionString, @"User\s*Id\s*=\s*([^;]+)");
                txtPassword.Text    = ExtractValue(connectionString, @"Password\s*=\s*([^;]+)");
            }
            finally
            {
                _isUpdatingFields = false;
            }
        }

        private string ExtractValue(string source, string pattern, string defaultValue = "")
        {
            if (string.IsNullOrWhiteSpace(source)) return string.Empty;
            var match = Regex.Match(source, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }

        // ── Rebuild Connection String Live ───────────────────────────────────────

        private void Parameter_TextChanged(object? sender, EventArgs e)
        {
            if (_isUpdatingFields) return;

            var host    = txtHost.Text.Trim();
            var port    = txtPort.Text.Trim();
            var service = txtServiceName.Text.Trim();
            var user    = txtUserId.Text.Trim();
            var pass    = txtPassword.Text;

            txtConnectionString.Text = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVER=dedicated)(SERVICE_NAME={service})));User Id={user};Password={pass};";
        }

        private void ChkShowPassword_CheckedChanged(object? sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        // ── Test Connection ──────────────────────────────────────────────────────

        private async void BtnTestConnection_Click(object? sender, EventArgs e)
        {
            var connectionString = txtConnectionString.Text.Trim();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                SetStatus(success: false, "✘ Please provide a valid Oracle connection string.");
                return;
            }

            btnTestConnection.Enabled = false;
            btnTestConnection.Text = "⏳ Testing...";
            SetStatus(success: true, "Attempting Oracle database connection...");

            try
            {
                using var conn = new OracleConnection(connectionString);
                await conn.OpenAsync();

                SetStatus(success: true, $"✔ Successfully connected to Oracle DB ({conn.ServerVersion})!");
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Connection failed: {ex.Message}");
            }
            finally
            {
                btnTestConnection.Enabled = true;
                btnTestConnection.Text = "🔌 Test Connection";
            }
        }

        // ── Save Connection Settings ─────────────────────────────────────────────

        private void BtnSaveDatabaseSettings_Click(object? sender, EventArgs e)
        {
            var connectionString = txtConnectionString.Text.Trim();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                SetStatus(success: false, "✘ Connection string cannot be empty.");
                return;
            }

            try
            {
                AppSettingsService.SaveConnectionString(connectionString);
                SetStatus(success: true, "✔ Connection settings saved successfully.");
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Save failed: {ex.Message}");
            }
        }

        // ── Status Helper ────────────────────────────────────────────────────────

        private void SetStatus(bool success, string message)
        {
            lblDatabaseSettingsStatus.ForeColor = success
                ? Color.FromArgb(22, 163, 74)   // Green
                : Color.FromArgb(185, 28, 28);  // Red
            lblDatabaseSettingsStatus.Text = message;
        }

        private void txtConnectionString_TextChanged(object? sender, EventArgs e)
        {
            if (_isUpdatingFields) return;
            var connectionString = txtConnectionString.Text.Trim();
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                ParseAndPopulateFields(connectionString);
            }
        }
    }
}
