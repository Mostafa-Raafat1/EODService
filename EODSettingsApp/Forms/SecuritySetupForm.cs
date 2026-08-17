using System;
using System.Drawing;
using System.Windows.Forms;
using EODService.Services;

namespace EODSettingsApp.Forms
{
    public partial class SecuritySetupForm : Form
    {
        public SecuritySetupForm()
        {
            InitializeComponent();
        }

        private void BtnSaveKey_Click(object? sender, EventArgs e)
        {
            var pass = txtPassphrase.Text;
            var confirm = txtConfirmPassphrase.Text;

            if (string.IsNullOrWhiteSpace(pass))
            {
                lblStatus.Text = "✘ Passphrase cannot be empty.";
                lblStatus.ForeColor = Color.Red;
                return;
            }

            if (pass.Length < 6)
            {
                lblStatus.Text = "✘ Passphrase must be at least 6 characters.";
                lblStatus.ForeColor = Color.Red;
                return;
            }

            if (pass != confirm)
            {
                lblStatus.Text = "✘ Passphrases do not match.";
                lblStatus.ForeColor = Color.Red;
                return;
            }

            try
            {
                KeyStoreService.SaveKey(pass);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✘ Error saving key: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }
    }
}
