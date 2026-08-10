using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class SymbolSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;

        // Main content controls
        private Panel pnlMain;
        private Label lblCurrentSymbols;
        private ListBox lstSymbols;
        private Button btnEditSymbol;
        private Button btnRemoveSymbol;
        private Label lblNewSymbol;
        private TextBox txtNewSymbol;
        private Button btnAddSymbol;

        // Footer
        private Panel pnlFooter;
        private Button btnSaveSymbolSettings;
        private Label lblSymbolSettingsStatus;

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
            lblCurrentSymbols = new Label();
            lstSymbols = new ListBox();
            btnEditSymbol = new Button();
            btnRemoveSymbol = new Button();
            lblNewSymbol = new Label();
            txtNewSymbol = new TextBox();
            btnAddSymbol = new Button();
            pnlFooter = new Panel();
            btnSaveSymbolSettings = new Button();
            lblSymbolSettingsStatus = new Label();
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
            pnlHeader.Size = new Size(460, 70);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(380, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Symbol Settings";
            // 
            // lblSubtitle
            // 
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubtitle.Location = new Point(22, 42);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(380, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "View, add, edit, and remove stock ticker symbols";
            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.FromArgb(248, 250, 252);
            pnlMain.Controls.Add(btnRemoveSymbol);
            pnlMain.Controls.Add(lblCurrentSymbols);
            pnlMain.Controls.Add(lstSymbols);
            pnlMain.Controls.Add(btnEditSymbol);
            pnlMain.Controls.Add(lblNewSymbol);
            pnlMain.Controls.Add(txtNewSymbol);
            pnlMain.Controls.Add(btnAddSymbol);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 70);
            pnlMain.Name = "pnlMain";
            pnlMain.Padding = new Padding(16);
            pnlMain.Size = new Size(460, 310);
            pnlMain.TabIndex = 1;
            // 
            // lblCurrentSymbols
            // 
            lblCurrentSymbols.AutoSize = true;
            lblCurrentSymbols.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblCurrentSymbols.ForeColor = Color.FromArgb(51, 65, 85);
            lblCurrentSymbols.Location = new Point(16, 12);
            lblCurrentSymbols.Name = "lblCurrentSymbols";
            lblCurrentSymbols.Size = new Size(156, 15);
            lblCurrentSymbols.TabIndex = 0;
            lblCurrentSymbols.Text = "Configured Stock Symbols:";
            // 
            // lstSymbols
            // 
            lstSymbols.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lstSymbols.ForeColor = Color.FromArgb(30, 41, 59);
            lstSymbols.FormattingEnabled = true;
            lstSymbols.Location = new Point(16, 32);
            lstSymbols.Name = "lstSymbols";
            lstSymbols.Size = new Size(280, 174);
            lstSymbols.TabIndex = 1;
            // 
            // btnEditSymbol
            // 
            btnEditSymbol.BackColor = Color.FromArgb(2, 132, 199);
            btnEditSymbol.Cursor = Cursors.Hand;
            btnEditSymbol.FlatAppearance.BorderSize = 0;
            btnEditSymbol.FlatStyle = FlatStyle.Flat;
            btnEditSymbol.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnEditSymbol.ForeColor = Color.White;
            btnEditSymbol.Location = new Point(310, 32);
            btnEditSymbol.Name = "btnEditSymbol";
            btnEditSymbol.Size = new Size(134, 28);
            btnEditSymbol.TabIndex = 2;
            btnEditSymbol.Text = "✏ Edit Selected";
            btnEditSymbol.UseVisualStyleBackColor = false;
            btnEditSymbol.Click += BtnEditSymbol_Click;
            // 
            // btnRemoveSymbol
            // 
            btnRemoveSymbol.BackColor = Color.FromArgb(225, 29, 72);
            btnRemoveSymbol.Cursor = Cursors.Hand;
            btnRemoveSymbol.FlatAppearance.BorderSize = 0;
            btnRemoveSymbol.FlatStyle = FlatStyle.Flat;
            btnRemoveSymbol.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnRemoveSymbol.ForeColor = Color.White;
            btnRemoveSymbol.Location = new Point(310, 68);
            btnRemoveSymbol.Name = "btnRemoveSymbol";
            btnRemoveSymbol.Size = new Size(134, 26);
            btnRemoveSymbol.TabIndex = 3;
            btnRemoveSymbol.Text = "🗑 Remove";
            btnRemoveSymbol.UseVisualStyleBackColor = false;
            btnRemoveSymbol.Click += BtnRemoveSymbol_Click;
            // 
            // lblNewSymbol
            // 
            lblNewSymbol.AutoSize = true;
            lblNewSymbol.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblNewSymbol.ForeColor = Color.FromArgb(51, 65, 85);
            lblNewSymbol.Location = new Point(16, 222);
            lblNewSymbol.Name = "lblNewSymbol";
            lblNewSymbol.Size = new Size(108, 15);
            lblNewSymbol.TabIndex = 4;
            lblNewSymbol.Text = "Add / Edit Symbol:";
            // 
            // txtNewSymbol
            // 
            txtNewSymbol.BorderStyle = BorderStyle.FixedSingle;
            txtNewSymbol.CharacterCasing = CharacterCasing.Upper;
            txtNewSymbol.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            txtNewSymbol.Location = new Point(16, 242);
            txtNewSymbol.Name = "txtNewSymbol";
            txtNewSymbol.Size = new Size(280, 25);
            txtNewSymbol.TabIndex = 5;
            txtNewSymbol.KeyDown += TxtNewSymbol_KeyDown;
            // 
            // btnAddSymbol
            // 
            btnAddSymbol.BackColor = Color.FromArgb(30, 58, 138);
            btnAddSymbol.Cursor = Cursors.Hand;
            btnAddSymbol.FlatAppearance.BorderSize = 0;
            btnAddSymbol.FlatStyle = FlatStyle.Flat;
            btnAddSymbol.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnAddSymbol.ForeColor = Color.White;
            btnAddSymbol.Location = new Point(310, 241);
            btnAddSymbol.Name = "btnAddSymbol";
            btnAddSymbol.Size = new Size(134, 27);
            btnAddSymbol.TabIndex = 6;
            btnAddSymbol.Text = "Add Symbol";
            btnAddSymbol.UseVisualStyleBackColor = false;
            btnAddSymbol.Click += BtnAddSymbol_Click;
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.FromArgb(226, 232, 240);
            pnlFooter.Controls.Add(btnSaveSymbolSettings);
            pnlFooter.Controls.Add(lblSymbolSettingsStatus);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 380);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(460, 60);
            pnlFooter.TabIndex = 2;
            // 
            // btnSaveSymbolSettings
            // 
            btnSaveSymbolSettings.BackColor = Color.FromArgb(30, 58, 138);
            btnSaveSymbolSettings.Cursor = Cursors.Hand;
            btnSaveSymbolSettings.FlatAppearance.BorderSize = 0;
            btnSaveSymbolSettings.FlatStyle = FlatStyle.Flat;
            btnSaveSymbolSettings.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSaveSymbolSettings.ForeColor = Color.White;
            btnSaveSymbolSettings.Location = new Point(268, 12);
            btnSaveSymbolSettings.Name = "btnSaveSymbolSettings";
            btnSaveSymbolSettings.Size = new Size(178, 36);
            btnSaveSymbolSettings.TabIndex = 0;
            btnSaveSymbolSettings.Text = "Save Symbol Settings";
            btnSaveSymbolSettings.UseVisualStyleBackColor = false;
            btnSaveSymbolSettings.Click += BtnSaveSymbolSettings_Click;
            // 
            // lblSymbolSettingsStatus
            // 
            lblSymbolSettingsStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblSymbolSettingsStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblSymbolSettingsStatus.Location = new Point(14, 20);
            lblSymbolSettingsStatus.Name = "lblSymbolSettingsStatus";
            lblSymbolSettingsStatus.Size = new Size(248, 22);
            lblSymbolSettingsStatus.TabIndex = 1;
            // 
            // SymbolSettingsForm
            // 
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(460, 440);
            Controls.Add(pnlMain);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(476, 479);
            MinimumSize = new Size(476, 479);
            Name = "SymbolSettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Symbol Settings";
            pnlHeader.ResumeLayout(false);
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
