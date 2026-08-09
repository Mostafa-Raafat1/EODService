using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel    pnlHeader;
        private Label    lblTitle;
        private Label    lblSubtitle;
        private Panel    pnlBody;
        private Label    lblProviderLabel;
        private ComboBox cmbProvider;
        private Label    lblProviderHint;
        private Panel    pnlFooter;
        private Button   btnSave;
        private Label    lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlBody = new Panel();
            lblProviderLabel = new Label();
            cmbProvider = new ComboBox();
            lblProviderHint = new Label();
            pnlFooter = new Panel();
            btnSave = new Button();
            lblStatus = new Label();
            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(30, 58, 138);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(460, 80);
            pnlHeader.TabIndex = 0;
            pnlHeader.Paint += pnlHeader_Paint;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(380, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "EOD Service Settings";
            lblTitle.Click += lblTitle_Click;
            // 
            // lblSubtitle
            // 
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubtitle.Location = new Point(22, 46);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(380, 22);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Configure your active data provider";
            // 
            // pnlBody
            // 
            pnlBody.BackColor = Color.FromArgb(245, 247, 250);
            pnlBody.Controls.Add(lblProviderLabel);
            pnlBody.Controls.Add(cmbProvider);
            pnlBody.Controls.Add(lblProviderHint);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 80);
            pnlBody.Name = "pnlBody";
            pnlBody.Size = new Size(460, 170);
            pnlBody.TabIndex = 1;
            // 
            // lblProviderLabel
            // 
            lblProviderLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblProviderLabel.ForeColor = Color.FromArgb(30, 41, 59);
            lblProviderLabel.Location = new Point(28, 30);
            lblProviderLabel.Name = "lblProviderLabel";
            lblProviderLabel.Size = new Size(200, 24);
            lblProviderLabel.TabIndex = 0;
            lblProviderLabel.Text = "Active Data Provider";
            // 
            // cmbProvider
            // 
            cmbProvider.BackColor = Color.White;
            cmbProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProvider.FlatStyle = FlatStyle.Flat;
            cmbProvider.Font = new Font("Segoe UI", 11F);
            cmbProvider.ForeColor = Color.FromArgb(15, 23, 42);
            cmbProvider.Items.AddRange(new object[] { "TwelveData", "Yahoo" });
            cmbProvider.Location = new Point(28, 60);
            cmbProvider.Name = "cmbProvider";
            cmbProvider.Size = new Size(400, 28);
            cmbProvider.TabIndex = 1;
            cmbProvider.SelectedIndexChanged += cmbProvider_SelectedIndexChanged_1;
            // 
            // lblProviderHint
            // 
            lblProviderHint.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            lblProviderHint.ForeColor = Color.FromArgb(100, 116, 139);
            lblProviderHint.Location = new Point(28, 98);
            lblProviderHint.Name = "lblProviderHint";
            lblProviderHint.Size = new Size(400, 20);
            lblProviderHint.TabIndex = 2;
            lblProviderHint.Text = "The selected provider will be used the next time EODService runs.";
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.FromArgb(226, 232, 240);
            pnlFooter.Controls.Add(btnSave);
            pnlFooter.Controls.Add(lblStatus);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 250);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(460, 70);
            pnlFooter.TabIndex = 2;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(30, 58, 138);
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(296, 15);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(150, 40);
            btnSave.TabIndex = 0;
            btnSave.Text = "Save Settings";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += BtnSave_Click;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblStatus.Location = new Point(16, 24);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(270, 22);
            lblStatus.TabIndex = 1;
            // 
            // SettingsForm
            // 
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(460, 320);
            Controls.Add(pnlBody);
            Controls.Add(pnlHeader);
            Controls.Add(pnlFooter);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(476, 359);
            MinimumSize = new Size(476, 359);
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EOD Service — Settings";
            Load += SettingsForm_Load;
            pnlHeader.ResumeLayout(false);
            pnlBody.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
