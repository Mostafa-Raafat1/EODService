using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class DataConnectionForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;

        private Label lblHost;
        private TextBox txtHost;

        private Label lblPort;
        private TextBox txtPort;

        private Label lblServiceName;
        private TextBox txtServiceName;

        private Label lblUsername;
        private TextBox txtUsername;

        private Label lblPassword;
        private TextBox txtPassword;

        private Label lblStatus;

        private Button btnTestConnection;
        private Button btnCancel;
        private Button btnSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();

            lblHost = new Label();
            txtHost = new TextBox();

            lblPort = new Label();
            txtPort = new TextBox();

            lblServiceName = new Label();
            txtServiceName = new TextBox();

            lblUsername = new Label();
            txtUsername = new TextBox();

            lblPassword = new Label();
            txtPassword = new TextBox();

            lblStatus = new Label();

            btnTestConnection = new Button();
            btnCancel = new Button();
            btnSave = new Button();

            pnlHeader.SuspendLayout();
            SuspendLayout();

            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(30, 58, 138);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(440, 55);
            pnlHeader.TabIndex = 0;

            // 
            // lblHeaderTitle
            // 
            lblHeaderTitle.Dock = DockStyle.Fill;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(0, 0);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(440, 55);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "Oracle Database Connection";
            lblHeaderTitle.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // lblHost
            // 
            lblHost.AutoSize = true;
            lblHost.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHost.ForeColor = Color.FromArgb(30, 41, 59);
            lblHost.Location = new Point(30, 75);
            lblHost.Name = "lblHost";
            lblHost.Size = new Size(81, 15);
            lblHost.TabIndex = 1;
            lblHost.Text = "Host / Server";

            // 
            // txtHost
            // 
            txtHost.Font = new Font("Segoe UI", 9.5F);
            txtHost.Location = new Point(30, 95);
            txtHost.Name = "txtHost";
            txtHost.Size = new Size(380, 24);
            txtHost.TabIndex = 2;
            txtHost.Text = "10.120.143.51";

            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPort.ForeColor = Color.FromArgb(30, 41, 59);
            lblPort.Location = new Point(30, 130);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(31, 15);
            lblPort.TabIndex = 3;
            lblPort.Text = "Port";

            // 
            // txtPort
            // 
            txtPort.Font = new Font("Segoe UI", 9.5F);
            txtPort.Location = new Point(30, 150);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(380, 24);
            txtPort.TabIndex = 4;
            txtPort.Text = "1521";

            // 
            // lblServiceName
            // 
            lblServiceName.AutoSize = true;
            lblServiceName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblServiceName.ForeColor = Color.FromArgb(30, 41, 59);
            lblServiceName.Location = new Point(30, 185);
            lblServiceName.Name = "lblServiceName";
            lblServiceName.Size = new Size(84, 15);
            lblServiceName.TabIndex = 5;
            lblServiceName.Text = "Service Name";

            // 
            // txtServiceName
            // 
            txtServiceName.Font = new Font("Segoe UI", 9.5F);
            txtServiceName.Location = new Point(30, 205);
            txtServiceName.Name = "txtServiceName";
            txtServiceName.Size = new Size(380, 24);
            txtServiceName.TabIndex = 6;
            txtServiceName.Text = "cibcorclhq";

            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUsername.ForeColor = Color.FromArgb(30, 41, 59);
            lblUsername.Location = new Point(30, 240);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(64, 15);
            lblUsername.TabIndex = 7;
            lblUsername.Text = "Username";

            // 
            // txtUsername
            // 
            txtUsername.Font = new Font("Segoe UI", 9.5F);
            txtUsername.Location = new Point(30, 260);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(380, 24);
            txtUsername.TabIndex = 8;
            txtUsername.Text = "intern";

            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(30, 41, 59);
            lblPassword.Location = new Point(30, 295);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(60, 15);
            lblPassword.TabIndex = 9;
            lblPassword.Text = "Password";

            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 9.5F);
            txtPassword.Location = new Point(30, 315);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(380, 24);
            txtPassword.TabIndex = 10;
            txtPassword.Text = "intern";
            txtPassword.UseSystemPasswordChar = true;

            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(71, 85, 105);
            lblStatus.Location = new Point(30, 350);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(380, 20);
            lblStatus.TabIndex = 11;
            lblStatus.Text = "● Connection Status: Not tested";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // btnTestConnection
            // 
            btnTestConnection.BackColor = Color.FromArgb(71, 85, 105);
            btnTestConnection.Cursor = Cursors.Hand;
            btnTestConnection.FlatAppearance.BorderSize = 0;
            btnTestConnection.FlatStyle = FlatStyle.Flat;
            btnTestConnection.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnTestConnection.ForeColor = Color.White;
            btnTestConnection.Location = new Point(130, 380);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(180, 36);
            btnTestConnection.TabIndex = 12;
            btnTestConnection.Text = "Test Connection";
            btnTestConnection.UseVisualStyleBackColor = false;
            btnTestConnection.Click += BtnTestConnection_Click;

            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(226, 232, 240);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(30, 41, 59);
            btnCancel.Location = new Point(130, 426);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(85, 36);
            btnCancel.TabIndex = 13;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += BtnCancel_Click;

            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(30, 58, 138);
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(225, 426);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(85, 36);
            btnSave.TabIndex = 14;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += BtnSave_Click;

            // 
            // DataConnectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(440, 485);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(btnTestConnection);
            Controls.Add(lblStatus);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtUsername);
            Controls.Add(lblUsername);
            Controls.Add(txtServiceName);
            Controls.Add(lblServiceName);
            Controls.Add(txtPort);
            Controls.Add(lblPort);
            Controls.Add(txtHost);
            Controls.Add(lblHost);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DataConnectionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Data Connection";
            pnlHeader.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
