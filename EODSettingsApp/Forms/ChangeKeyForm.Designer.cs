namespace EODSettingsApp.Forms
{
    partial class ChangeKeyForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.TextBox txtCurrentPassphrase;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.TextBox txtNewPassphrase;
        private System.Windows.Forms.Label lblConfirm;
        private System.Windows.Forms.TextBox txtConfirmNewPassphrase;
        private System.Windows.Forms.Button btnChangeKey;
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
            this.lblCurrent = new System.Windows.Forms.Label();
            this.txtCurrentPassphrase = new System.Windows.Forms.TextBox();
            this.lblNew = new System.Windows.Forms.Label();
            this.txtNewPassphrase = new System.Windows.Forms.TextBox();
            this.lblConfirm = new System.Windows.Forms.Label();
            this.txtConfirmNewPassphrase = new System.Windows.Forms.TextBox();
            this.btnChangeKey = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lblInfo
            this.lblInfo.Location = new System.Drawing.Point(20, 15);
            this.lblInfo.Size = new System.Drawing.Size(420, 45);
            this.lblInfo.Text = "🔑 Change Encryption Passphrase\n\nThis will safely re-encrypt all stored database credentials with your new passphrase.";
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);

            // lblCurrent
            this.lblCurrent.Location = new System.Drawing.Point(20, 65);
            this.lblCurrent.Size = new System.Drawing.Size(180, 20);
            this.lblCurrent.Text = "Current Passphrase:";

            // txtCurrentPassphrase
            this.txtCurrentPassphrase.Location = new System.Drawing.Point(20, 85);
            this.txtCurrentPassphrase.Size = new System.Drawing.Size(420, 25);
            this.txtCurrentPassphrase.UseSystemPasswordChar = true;

            // lblNew
            this.lblNew.Location = new System.Drawing.Point(20, 120);
            this.lblNew.Size = new System.Drawing.Size(180, 20);
            this.lblNew.Text = "New Passphrase:";

            // txtNewPassphrase
            this.txtNewPassphrase.Location = new System.Drawing.Point(20, 140);
            this.txtNewPassphrase.Size = new System.Drawing.Size(420, 25);
            this.txtNewPassphrase.UseSystemPasswordChar = true;

            // lblConfirm
            this.lblConfirm.Location = new System.Drawing.Point(20, 175);
            this.lblConfirm.Size = new System.Drawing.Size(180, 20);
            this.lblConfirm.Text = "Confirm New Passphrase:";

            // txtConfirmNewPassphrase
            this.txtConfirmNewPassphrase.Location = new System.Drawing.Point(20, 195);
            this.txtConfirmNewPassphrase.Size = new System.Drawing.Size(420, 25);
            this.txtConfirmNewPassphrase.UseSystemPasswordChar = true;

            // btnChangeKey
            this.btnChangeKey.Location = new System.Drawing.Point(20, 235);
            this.btnChangeKey.Size = new System.Drawing.Size(420, 35);
            this.btnChangeKey.Text = "🔒 Re-Encrypt Database && Save New Key";
            this.btnChangeKey.UseVisualStyleBackColor = true;
            this.btnChangeKey.Click += new System.EventHandler(this.BtnChangeKey_Click);

            // lblStatus
            this.lblStatus.Location = new System.Drawing.Point(20, 280);
            this.lblStatus.Size = new System.Drawing.Size(420, 30);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ChangeKeyForm
            this.ClientSize = new System.Drawing.Size(460, 320);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblCurrent);
            this.Controls.Add(this.txtCurrentPassphrase);
            this.Controls.Add(this.lblNew);
            this.Controls.Add(this.txtNewPassphrase);
            this.Controls.Add(this.lblConfirm);
            this.Controls.Add(this.txtConfirmNewPassphrase);
            this.Controls.Add(this.btnChangeKey);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EODService — Change Encryption Key";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
