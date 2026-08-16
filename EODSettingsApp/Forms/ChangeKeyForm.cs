using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EODService.DTOs.OracleSettings;
using EODService.Persistance;
using EODService.Persistance.Repo;
using EODService.Services;

namespace EODSettingsApp.Forms
{
    public partial class ChangeKeyForm : Form
    {
        public ChangeKeyForm()
        {
            InitializeComponent();
        }

        private async void BtnChangeKey_Click(object? sender, EventArgs e)
        {
            var currentPass = txtCurrentPassphrase.Text;
            var newPass     = txtNewPassphrase.Text;
            var confirmPass = txtConfirmNewPassphrase.Text;

            if (string.IsNullOrWhiteSpace(currentPass))
            {
                SetStatus(false, "✘ Please enter your current passphrase.");
                return;
            }

            if (!KeyStoreService.VerifyPassphrase(currentPass))
            {
                SetStatus(false, "✘ Current passphrase is incorrect.");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPass))
            {
                SetStatus(false, "✘ New passphrase cannot be empty.");
                return;
            }

            if (newPass.Length < 6)
            {
                SetStatus(false, "✘ New passphrase must be at least 6 characters.");
                return;
            }

            if (newPass != confirmPass)
            {
                SetStatus(false, "✘ New passphrases do not match.");
                return;
            }

            btnChangeKey.Enabled = false;
            SetStatus(true, "⌛ Re-encrypting database credentials with new key...");

            try
            {
                var connectionString = OracleSettingsMapper.GetConnectionString();
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    SetStatus(false, "✘ Database connection string not found.");
                    return;
                }

                // 1. Fetch current API Key from Oracle DB using current key (decrypted into memory)
                using var dbContext = AppDbContextFactory.Create(connectionString);
                IProvider repo = new ProviderRepo(dbContext);
                var twelveDb = await repo.GetProviderById(2);

                string? plainTextApiKey = twelveDb?.ApiKey;

                // 2. Update local key store file to the NEW passphrase
                KeyStoreService.SaveKey(newPass);

                // 3. Re-encrypt API key in Oracle DB using the NEW passphrase via repo.UpdateProvider
                if (twelveDb != null && !string.IsNullOrEmpty(plainTextApiKey))
                {
                    await repo.UpdateProvider(2, twelveDb.Name, twelveDb.BaseUrl, twelveDb.EndPoint, plainTextApiKey);
                }

                MessageBox.Show("Encryption passphrase updated and database values re-encrypted successfully! 🔒",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                SetStatus(false, $"✘ Error during re-keying: {ex.Message}");
            }
            finally
            {
                btnChangeKey.Enabled = true;
            }
        }

        private void SetStatus(bool success, string message)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = success ? Color.DarkGreen : Color.Red;
        }
    }
}
