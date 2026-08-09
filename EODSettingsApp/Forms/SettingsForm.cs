using System;
using System.Windows.Forms;
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
        }

        // ── Active-provider dropdown ─────────────────────────────────────────────

        /// <summary>
        /// On startup: reads the external config and pre-selects the active provider.
        /// </summary>
        private void LoadCurrentSettings()
        {
            try
            {
                var settings = ExternalSettingsService.Load();
                var index    = cmbProvider.Items.IndexOf(settings.ProviderSettings.ActiveProvider);
                cmbProvider.SelectedIndex = index >= 0 ? index : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not load active-provider setting:\n{ex.Message}",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbProvider.SelectedIndex = 0; // safe default
            }
        }

        /// <summary>
        /// Validates selection → saves active provider → launches EODService.
        /// </summary>
        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (!TryGetSelectedProvider(out var selectedProvider))
                return;

            if (!TrySaveActiveProvider(selectedProvider))
                return;

            TryLaunchEodService(selectedProvider);
        }

        private bool TryGetSelectedProvider(out string provider)
        {
            provider = string.Empty;

            if (cmbProvider.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a provider before saving.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            provider = cmbProvider.SelectedItem.ToString()!;
            return true;
        }

        private bool TrySaveActiveProvider(string selectedProvider)
        {
            try
            {
                ExternalSettingsService.Save(new ExternalSettings
                {
                    ProviderSettings = new ProviderSettingsSection
                    {
                        ActiveProvider = selectedProvider
                    }
                });

                SetStatus(success: true,
                    $"✔  Saved! Active Provider set to '{selectedProvider}'.");
                return true;
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘  Save failed: {ex.Message}");
                return false;
            }
        }

        private void TryLaunchEodService(string selectedProvider)
        {
            try
            {
                var exePath = EodServiceLauncher.ResolveExePath();
                EodServiceLauncher.Launch(exePath);

                SetStatus(success: true,
                    $"✔  Saved & launched EODService (provider: {selectedProvider}).");
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘  Launch failed: {ex.Message}");
            }
        }

        private void SetStatus(bool success, string message)
        {
            lblStatus.ForeColor = success
                ? System.Drawing.Color.FromArgb(22, 163, 74)
                : System.Drawing.Color.FromArgb(185, 28, 28);
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

        // ── Unused designer event stubs ──────────────────────────────────────────

        private void cmbProvider_SelectedIndexChanged(object sender, EventArgs e) { }
        private void SettingsForm_Load(object sender, EventArgs e) { }
        private void lblTitle_Click(object sender, EventArgs e) { }
        private void pnlHeader_Paint(object sender, PaintEventArgs e) { }
        private void cmbProvider_SelectedIndexChanged_1(object sender, EventArgs e) { }
    }
}
