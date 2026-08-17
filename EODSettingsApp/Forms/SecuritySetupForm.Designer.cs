namespace EODSettingsApp.Forms
{
    partial class SecuritySetupForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblPassphrase;
        private System.Windows.Forms.TextBox txtPassphrase;
        private System.Windows.Forms.Label lblConfirm;
        private System.Windows.Forms.TextBox txtConfirmPassphrase;
        private System.Windows.Forms.Button btnSaveKey;
        private System.Windows.Forms.Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblPassphrase = new System.Windows.Forms.Label();
            this.txtPassphrase = new System.Windows.Forms.TextBox();
            this.lblConfirm = new System.Windows.Forms.Label();
            this.txtConfirmPassphrase = new System.Windows.Forms.TextBox();
            this.btnSaveKey = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lblInfo
            this.lblInfo.Location = new System.Drawing.Point(20, 15);
            this.lblInfo.Size = new System.Drawing.Size(420, 50);
            this.lblInfo.Text = "🔒 Initial Security Setup\n\nEnter a shared encryption passphrase. Make sure to use the SAME passphrase on all devices running EODService so shared database data can be decrypted.";
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);

            // lblPassphrase
            this.lblPassphrase.Location = new System.Drawing.Point(20, 75);
            this.lblPassphrase.Size = new System.Drawing.Size(150, 20);
            this.lblPassphrase.Text = "Encryption Passphrase:";

            // txtPassphrase
            this.txtPassphrase.Location = new System.Drawing.Point(20, 95);
            this.txtPassphrase.Size = new System.Drawing.Size(420, 25);
            this.txtPassphrase.UseSystemPasswordChar = true;

            // lblConfirm
            this.lblConfirm.Location = new System.Drawing.Point(20, 130);
            this.lblConfirm.Size = new System.Drawing.Size(150, 20);
            this.lblConfirm.Text = "Confirm Passphrase:";

            // txtConfirmPassphrase
            this.txtConfirmPassphrase.Location = new System.Drawing.Point(20, 150);
            this.txtConfirmPassphrase.Size = new System.Drawing.Size(420, 25);
            this.txtConfirmPassphrase.UseSystemPasswordChar = true;

            // btnSaveKey
            this.btnSaveKey.Location = new System.Drawing.Point(20, 195);
            this.btnSaveKey.Size = new System.Drawing.Size(420, 35);
            this.btnSaveKey.Text = "Save Key && Initialize Security";
            this.btnSaveKey.UseVisualStyleBackColor = true;
            this.btnSaveKey.Click += new System.EventHandler(this.BtnSaveKey_Click);

            // lblStatus
            this.lblStatus.Location = new System.Drawing.Point(20, 240);
            this.lblStatus.Size = new System.Drawing.Size(420, 30);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // SecuritySetupForm
            this.ClientSize = new System.Drawing.Size(460, 280);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblPassphrase);
            this.Controls.Add(this.txtPassphrase);
            this.Controls.Add(this.lblConfirm);
            this.Controls.Add(this.txtConfirmPassphrase);
            this.Controls.Add(this.btnSaveKey);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EODService — Encryption Key Setup";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
