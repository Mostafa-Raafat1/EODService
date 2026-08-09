using System;
using System.Windows.Forms;
using EODSettingsApp.ExternalConfig;

namespace EODSettingsApp.Forms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        /// <summary>
        /// On startup: read the external config and set the dropdown
        /// to reflect the currently active provider.
        /// </summary>
        private void LoadCurrentSettings()
        {
            try
            {
                var settings = ExternalSettingsService.Load();
                var currentProvider = settings.ProviderSettings.ActiveProvider;

                // Select the right item in the dropdown
                var index = cmbProvider.Items.IndexOf(currentProvider);
                cmbProvider.SelectedIndex = index >= 0 ? index : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not load settings:\n{ex.Message}",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbProvider.SelectedIndex = 0; // safe default
            }
        }

        /// <summary>
        /// On Save: write the selected provider to C:\EODConfig\settings.json.
        /// </summary>
        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cmbProvider.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select a provider before saving.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var selectedProvider = cmbProvider.SelectedItem.ToString()!;

                var settings = new ExternalSettings
                {
                    ProviderSettings = new ProviderSettingsSection
                    {
                        ActiveProvider = selectedProvider
                    }
                };

                ExternalSettingsService.Save(settings);

                // Show success feedback in the status label
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(22, 163, 74); // green
                lblStatus.Text = $"✔  Saved! Active Provider set to '{selectedProvider}'.";
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28); // red
                lblStatus.Text = $"✘  Error: {ex.Message}";
            }
        }

        private void cmbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbProvider_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
