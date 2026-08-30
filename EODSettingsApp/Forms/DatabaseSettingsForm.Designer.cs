using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class DatabaseSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;

        // Main content controls
        private Panel pnlMain;
        private Label lblHost;
        private TextBox txtHost;
        private Label lblPort;
        private TextBox txtPort;
        private Label lblServiceName;
        private TextBox txtServiceName;
        private Label lblUserId;
        private TextBox txtUserId;
        private Label lblPassword;
        private TextBox txtPassword;
        private CheckBox chkShowPassword;
        private Label lblConnectionString;
        private TextBox txtConnectionString;
        private Button btnTestConnection;

        // Footer
        private Panel pnlFooter;
        private Button btnSaveDatabaseSettings;
        private Label lblDatabaseSettingsStatus;

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
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlMain = new Panel();
            lblHost = new Label();
            txtHost = new TextBox();
            lblPort = new Label();
            txtPort = new TextBox();
            lblServiceName = new Label();
            txtServiceName = new TextBox();
            lblUserId = new Label();
            txtUserId = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            chkShowPassword = new CheckBox();
            lblConnectionString = new Label();
            txtConnectionString = new TextBox();
            btnTestConnection = new Button();
            pnlFooter = new Panel();
            btnSaveDatabaseSettings = new Button();
            lblDatabaseSettingsStatus = new Label();
            pnlHeader.SuspendLayout();
            pnlMain.SuspendLayout();
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
            pnlHeader.Size = new Size(544, 70);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(460, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Database Connection Settings";
            // 
            // lblSubtitle
            // 
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubtitle.Location = new Point(22, 42);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(460, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Configure Oracle DB server host, port, credentials, and test connectivity";
            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.FromArgb(248, 250, 252);
            pnlMain.Controls.Add(lblHost);
            pnlMain.Controls.Add(txtHost);
            pnlMain.Controls.Add(lblPort);
            pnlMain.Controls.Add(txtPort);
            pnlMain.Controls.Add(lblServiceName);
            pnlMain.Controls.Add(txtServiceName);
            pnlMain.Controls.Add(lblUserId);
            pnlMain.Controls.Add(txtUserId);
            pnlMain.Controls.Add(lblPassword);
            pnlMain.Controls.Add(txtPassword);
            pnlMain.Controls.Add(chkShowPassword);
            pnlMain.Controls.Add(lblConnectionString);
            pnlMain.Controls.Add(txtConnectionString);
            pnlMain.Controls.Add(btnTestConnection);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 70);
            pnlMain.Name = "pnlMain";
            pnlMain.Padding = new Padding(20);
            pnlMain.Size = new Size(544, 410);
            pnlMain.TabIndex = 1;
            // 
            // lblHost
            // 
            lblHost.AutoSize = true;
            lblHost.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblHost.ForeColor = Color.FromArgb(51, 65, 85);
            lblHost.Location = new Point(20, 16);
            lblHost.Name = "lblHost";
            lblHost.Size = new Size(105, 15);
            lblHost.TabIndex = 0;
            lblHost.Text = "Host IP / Address:";
            // 
            // txtHost
            // 
            txtHost.Font = new Font("Segoe UI", 9.5F);
            txtHost.Location = new Point(20, 36);
            txtHost.Name = "txtHost";
            txtHost.Size = new Size(330, 24);
            txtHost.TabIndex = 0;
            txtHost.TextChanged += Parameter_TextChanged;
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblPort.ForeColor = Color.FromArgb(51, 65, 85);
            lblPort.Location = new Point(366, 16);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(34, 15);
            lblPort.TabIndex = 1;
            lblPort.Text = "Port:";
            // 
            // txtPort
            // 
            txtPort.Font = new Font("Segoe UI", 9.5F);
            txtPort.Location = new Point(366, 36);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(155, 24);
            txtPort.TabIndex = 1;
            txtPort.TextChanged += Parameter_TextChanged;
            // 
            // lblServiceName
            // 
            lblServiceName.AutoSize = true;
            lblServiceName.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblServiceName.ForeColor = Color.FromArgb(51, 65, 85);
            lblServiceName.Location = new Point(20, 74);
            lblServiceName.Name = "lblServiceName";
            lblServiceName.Size = new Size(88, 15);
            lblServiceName.TabIndex = 2;
            lblServiceName.Text = "Service Name:";
            // 
            // txtServiceName
            // 
            txtServiceName.Font = new Font("Segoe UI", 9.5F);
            txtServiceName.Location = new Point(20, 94);
            txtServiceName.Name = "txtServiceName";
            txtServiceName.Size = new Size(501, 24);
            txtServiceName.TabIndex = 2;
            txtServiceName.TextChanged += Parameter_TextChanged;
            // 
            // lblUserId
            // 
            lblUserId.AutoSize = true;
            lblUserId.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblUserId.ForeColor = Color.FromArgb(51, 65, 85);
            lblUserId.Location = new Point(20, 132);
            lblUserId.Name = "lblUserId";
            lblUserId.Size = new Size(50, 15);
            lblUserId.TabIndex = 3;
            lblUserId.Text = "User Id:";
            // 
            // txtUserId
            // 
            txtUserId.Font = new Font("Segoe UI", 9.5F);
            txtUserId.Location = new Point(20, 152);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(240, 24);
            txtUserId.TabIndex = 3;
            txtUserId.TextChanged += Parameter_TextChanged;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(51, 65, 85);
            lblPassword.Location = new Point(281, 132);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(62, 15);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 9.5F);
            txtPassword.Location = new Point(281, 152);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(240, 24);
            txtPassword.TabIndex = 4;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.TextChanged += Parameter_TextChanged;
            // 
            // chkShowPassword
            // 
            chkShowPassword.AutoSize = true;
            chkShowPassword.Font = new Font("Segoe UI", 8F);
            chkShowPassword.ForeColor = Color.FromArgb(71, 85, 105);
            chkShowPassword.Location = new Point(281, 180);
            chkShowPassword.Name = "chkShowPassword";
            chkShowPassword.Size = new Size(107, 17);
            chkShowPassword.TabIndex = 5;
            chkShowPassword.Text = "Show Password";
            chkShowPassword.UseVisualStyleBackColor = true;
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;
            // 
            // lblConnectionString
            // 
            lblConnectionString.AutoSize = true;
            lblConnectionString.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblConnectionString.ForeColor = Color.FromArgb(51, 65, 85);
            lblConnectionString.Location = new Point(20, 212);
            lblConnectionString.Name = "lblConnectionString";
            lblConnectionString.Size = new Size(132, 15);
            lblConnectionString.TabIndex = 6;
            lblConnectionString.Text = "Full Connection String:";
            // 
            // txtConnectionString
            // 
            txtConnectionString.Font = new Font("Consolas", 8.5F);
            txtConnectionString.ForeColor = Color.FromArgb(30, 41, 59);
            txtConnectionString.Location = new Point(20, 232);
            txtConnectionString.Multiline = true;
            txtConnectionString.Name = "txtConnectionString";
            txtConnectionString.ScrollBars = ScrollBars.Vertical;
            txtConnectionString.Size = new Size(501, 70);
            txtConnectionString.TabIndex = 6;
            txtConnectionString.TextChanged += txtConnectionString_TextChanged;
            // 
            // btnTestConnection
            // 
            btnTestConnection.BackColor = Color.FromArgb(2, 132, 199);
            btnTestConnection.Cursor = Cursors.Hand;
            btnTestConnection.FlatAppearance.BorderSize = 0;
            btnTestConnection.FlatStyle = FlatStyle.Flat;
            btnTestConnection.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnTestConnection.ForeColor = Color.White;
            btnTestConnection.Location = new Point(351, 312);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(170, 32);
            btnTestConnection.TabIndex = 7;
            btnTestConnection.Text = "🔌 Test Connection";
            btnTestConnection.UseVisualStyleBackColor = false;
            btnTestConnection.Click += BtnTestConnection_Click;
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.FromArgb(226, 232, 240);
            pnlFooter.Controls.Add(btnSaveDatabaseSettings);
            pnlFooter.Controls.Add(lblDatabaseSettingsStatus);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 480);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(544, 60);
            pnlFooter.TabIndex = 2;
            // 
            // btnSaveDatabaseSettings
            // 
            btnSaveDatabaseSettings.BackColor = Color.FromArgb(30, 58, 138);
            btnSaveDatabaseSettings.Cursor = Cursors.Hand;
            btnSaveDatabaseSettings.FlatAppearance.BorderSize = 0;
            btnSaveDatabaseSettings.FlatStyle = FlatStyle.Flat;
            btnSaveDatabaseSettings.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSaveDatabaseSettings.ForeColor = Color.White;
            btnSaveDatabaseSettings.Location = new Point(330, 12);
            btnSaveDatabaseSettings.Name = "btnSaveDatabaseSettings";
            btnSaveDatabaseSettings.Size = new Size(191, 36);
            btnSaveDatabaseSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveDatabaseSettings.TabIndex = 0;
            btnSaveDatabaseSettings.Text = "Save Connection Settings";
            btnSaveDatabaseSettings.UseVisualStyleBackColor = false;
            btnSaveDatabaseSettings.Click += BtnSaveDatabaseSettings_Click;
            // 
            // lblDatabaseSettingsStatus
            // 
            lblDatabaseSettingsStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblDatabaseSettingsStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblDatabaseSettingsStatus.Location = new Point(16, 16);
            lblDatabaseSettingsStatus.Name = "lblDatabaseSettingsStatus";
            lblDatabaseSettingsStatus.Size = new Size(308, 30);
            lblDatabaseSettingsStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDatabaseSettingsStatus.TabIndex = 1;
            // 
            // DatabaseSettingsForm
            // 
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(544, 540);
            Controls.Add(pnlMain);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(560, 579);
            MinimumSize = new Size(560, 579);
            Name = "DatabaseSettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "TICKR";
            pnlHeader.ResumeLayout(false);
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
