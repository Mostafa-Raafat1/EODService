using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel        pnlHeader;
        private Label        lblTitle;
        private Label        lblSubtitle;
        private Panel        pnlControls;
        private Label        lblProviderLabel;
        private ComboBox     cmbProvider;
        private Label        lblProviderHint;
        private Label        lblStatus;
        private Panel        pnlGrid;       // outer wrapper (DockStyle.Fill)
        private Panel        pnlGridHeader; // top strip inside pnlGrid
        private Label        lblGridTitle;
        private DataGridView dgvResults;    // fills the rest of pnlGrid
        
        private Panel        pnlLogs;
        private RichTextBox  rtbLogs;

        // Automated Schedule Controls
        private CheckBox     chkEnableSchedule;
        private Label        lblWorkingDaysLabel;
        private CheckBox     chkMon;
        private CheckBox     chkTue;
        private CheckBox     chkWed;
        private CheckBox     chkThu;
        private CheckBox     chkFri;
        private CheckBox     chkSat;
        private CheckBox     chkSun;
        private Label        lblTimeLabel;
        private DateTimePicker dtpRunTime;
        private Button       btnSaveSchedule;
        private Label        lblNextRunStatus;

        // Menu bar
        private MenuStrip         mnuMain;
        private ToolStripMenuItem mnuItemSettings;
        private ToolStripMenuItem mnuItemProviderSettings;
        private ToolStripMenuItem mnuItemSymbolSettings;
        private ToolStripMenuItem mnuItemDatabaseSettings;
        private ToolStripMenuItem mnuItemHistory;
        private ToolStripMenuItem mnuItemAddStock;

        // Toolbar
        private ToolStrip         tsToolBar;
        private ToolStripButton   tsBtnRunNow;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlControls = new Panel();
            lblProviderLabel = new Label();
            cmbProvider = new ComboBox();
            lblProviderHint = new Label();
            chkEnableSchedule = new CheckBox();
            lblWorkingDaysLabel = new Label();
            chkMon = new CheckBox();
            chkTue = new CheckBox();
            chkWed = new CheckBox();
            chkThu = new CheckBox();
            chkFri = new CheckBox();
            chkSat = new CheckBox();
            chkSun = new CheckBox();
            lblTimeLabel = new Label();
            dtpRunTime = new DateTimePicker();
            btnSaveSchedule = new Button();
            lblNextRunStatus = new Label();
            lblStatus = new Label();
            pnlGrid = new Panel();
            dgvResults = new DataGridView();
            pnlGridHeader = new Panel();
            lblGridTitle = new Label();
            pnlLogs = new Panel();
            rtbLogs = new RichTextBox();
            mnuMain = new MenuStrip();
            mnuItemSettings = new ToolStripMenuItem();
            mnuItemProviderSettings = new ToolStripMenuItem();
            mnuItemSymbolSettings = new ToolStripMenuItem();
            mnuItemDatabaseSettings = new ToolStripMenuItem();
            mnuItemAddStock = new ToolStripMenuItem();
            mnuItemHistory = new ToolStripMenuItem();
            tsToolBar = new ToolStrip();
            tsBtnRunNow = new ToolStripButton();
            pnlHeader.SuspendLayout();
            pnlControls.SuspendLayout();
            pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvResults).BeginInit();
            pnlGridHeader.SuspendLayout();
            pnlLogs.SuspendLayout();
            mnuMain.SuspendLayout();
            tsToolBar.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(30, 58, 138);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 24);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1034, 75);
            pnlHeader.TabIndex = 2;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(600, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "EOD Data Service";
            // 
            // lblSubtitle
            // 
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubtitle.Location = new Point(22, 44);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(700, 22);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Configure active data provider, working days schedule, and execution times";
            // 
            // pnlControls
            // 
            pnlControls.BackColor = Color.FromArgb(235, 238, 245);
            pnlControls.Controls.Add(lblProviderLabel);
            pnlControls.Controls.Add(cmbProvider);
            pnlControls.Controls.Add(lblProviderHint);
            pnlControls.Controls.Add(chkEnableSchedule);
            pnlControls.Controls.Add(lblWorkingDaysLabel);
            pnlControls.Controls.Add(chkMon);
            pnlControls.Controls.Add(chkTue);
            pnlControls.Controls.Add(chkWed);
            pnlControls.Controls.Add(chkThu);
            pnlControls.Controls.Add(chkFri);
            pnlControls.Controls.Add(chkSat);
            pnlControls.Controls.Add(chkSun);
            pnlControls.Controls.Add(lblTimeLabel);
            pnlControls.Controls.Add(dtpRunTime);
            pnlControls.Controls.Add(btnSaveSchedule);
            pnlControls.Controls.Add(lblNextRunStatus);
            pnlControls.Controls.Add(lblStatus);
            pnlControls.Dock = DockStyle.Top;
            pnlControls.Location = new Point(0, 99);
            pnlControls.Name = "pnlControls";
            pnlControls.Size = new Size(1034, 115);
            pnlControls.TabIndex = 1;
            // 
            // lblProviderLabel
            // 
            lblProviderLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblProviderLabel.ForeColor = Color.FromArgb(30, 41, 59);
            lblProviderLabel.Location = new Point(20, 14);
            lblProviderLabel.Name = "lblProviderLabel";
            lblProviderLabel.Size = new Size(150, 22);
            lblProviderLabel.TabIndex = 0;
            lblProviderLabel.Text = "Active Provider:";
            // 
            // cmbProvider
            // 
            cmbProvider.BackColor = Color.White;
            cmbProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProvider.FlatStyle = FlatStyle.Flat;
            cmbProvider.Font = new Font("Segoe UI", 9.5F);
            cmbProvider.ForeColor = Color.FromArgb(15, 23, 42);
            cmbProvider.Items.AddRange(new object[] { "TwelveData", "Yahoo" });
            cmbProvider.Location = new Point(170, 11);
            cmbProvider.Name = "cmbProvider";
            cmbProvider.Size = new Size(160, 25);
            cmbProvider.TabIndex = 0;
            cmbProvider.SelectedIndexChanged += cmbProvider_SelectedIndexChanged;
            // 
            // lblProviderHint
            // 
            lblProviderHint.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblProviderHint.ForeColor = Color.FromArgb(100, 116, 139);
            lblProviderHint.Location = new Point(20, 39);
            lblProviderHint.Name = "lblProviderHint";
            lblProviderHint.Size = new Size(310, 18);
            lblProviderHint.TabIndex = 1;
            lblProviderHint.Text = "Saves provider and updates automated execution.";
            // 
            // chkEnableSchedule
            // 
            chkEnableSchedule.AutoSize = true;
            chkEnableSchedule.Checked = true;
            chkEnableSchedule.CheckState = CheckState.Checked;
            chkEnableSchedule.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkEnableSchedule.ForeColor = Color.FromArgb(30, 58, 138);
            chkEnableSchedule.Location = new Point(360, 14);
            chkEnableSchedule.Name = "chkEnableSchedule";
            chkEnableSchedule.Size = new Size(182, 19);
            chkEnableSchedule.TabIndex = 2;
            chkEnableSchedule.Text = "Enable Automated Schedule";
            chkEnableSchedule.UseVisualStyleBackColor = true;
            chkEnableSchedule.CheckedChanged += ChkEnableSchedule_CheckedChanged;
            // 
            // lblWorkingDaysLabel
            // 
            lblWorkingDaysLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblWorkingDaysLabel.ForeColor = Color.FromArgb(51, 65, 85);
            lblWorkingDaysLabel.Location = new Point(360, 42);
            lblWorkingDaysLabel.Name = "lblWorkingDaysLabel";
            lblWorkingDaysLabel.Size = new Size(90, 20);
            lblWorkingDaysLabel.TabIndex = 3;
            lblWorkingDaysLabel.Text = "Working Days:";
            // 
            // chkMon
            // 
            chkMon.AutoSize = true;
            chkMon.Checked = true;
            chkMon.CheckState = CheckState.Checked;
            chkMon.Location = new Point(455, 42);
            chkMon.Name = "chkMon";
            chkMon.Size = new Size(54, 21);
            chkMon.TabIndex = 4;
            chkMon.Text = "Mon";
            chkMon.UseVisualStyleBackColor = true;
            // 
            // chkTue
            // 
            chkTue.AutoSize = true;
            chkTue.Checked = true;
            chkTue.CheckState = CheckState.Checked;
            chkTue.Location = new Point(510, 42);
            chkTue.Name = "chkTue";
            chkTue.Size = new Size(48, 21);
            chkTue.TabIndex = 5;
            chkTue.Text = "Tue";
            chkTue.UseVisualStyleBackColor = true;
            // 
            // chkWed
            // 
            chkWed.AutoSize = true;
            chkWed.Checked = true;
            chkWed.CheckState = CheckState.Checked;
            chkWed.Location = new Point(560, 42);
            chkWed.Name = "chkWed";
            chkWed.Size = new Size(53, 21);
            chkWed.TabIndex = 6;
            chkWed.Text = "Wed";
            chkWed.UseVisualStyleBackColor = true;
            // 
            // chkThu
            // 
            chkThu.AutoSize = true;
            chkThu.Checked = true;
            chkThu.CheckState = CheckState.Checked;
            chkThu.Location = new Point(615, 42);
            chkThu.Name = "chkThu";
            chkThu.Size = new Size(48, 21);
            chkThu.TabIndex = 7;
            chkThu.Text = "Thu";
            chkThu.UseVisualStyleBackColor = true;
            // 
            // chkFri
            // 
            chkFri.AutoSize = true;
            chkFri.Checked = true;
            chkFri.CheckState = CheckState.Checked;
            chkFri.Location = new Point(665, 42);
            chkFri.Name = "chkFri";
            chkFri.Size = new Size(41, 21);
            chkFri.TabIndex = 8;
            chkFri.Text = "Fri";
            chkFri.UseVisualStyleBackColor = true;
            // 
            // chkSat
            // 
            chkSat.AutoSize = true;
            chkSat.Location = new Point(710, 42);
            chkSat.Name = "chkSat";
            chkSat.Size = new Size(45, 21);
            chkSat.TabIndex = 9;
            chkSat.Text = "Sat";
            chkSat.UseVisualStyleBackColor = true;
            // 
            // chkSun
            // 
            chkSun.AutoSize = true;
            chkSun.Location = new Point(755, 42);
            chkSun.Name = "chkSun";
            chkSun.Size = new Size(48, 21);
            chkSun.TabIndex = 10;
            chkSun.Text = "Sun";
            chkSun.UseVisualStyleBackColor = true;
            // 
            // lblTimeLabel
            // 
            lblTimeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTimeLabel.ForeColor = Color.FromArgb(51, 65, 85);
            lblTimeLabel.Location = new Point(555, 14);
            lblTimeLabel.Name = "lblTimeLabel";
            lblTimeLabel.Size = new Size(65, 20);
            lblTimeLabel.TabIndex = 11;
            lblTimeLabel.Text = "Run Time:";
            // 
            // dtpRunTime
            // 
            dtpRunTime.CustomFormat = "HH:mm";
            dtpRunTime.Format = DateTimePickerFormat.Custom;
            dtpRunTime.Location = new Point(625, 11);
            dtpRunTime.Name = "dtpRunTime";
            dtpRunTime.ShowUpDown = true;
            dtpRunTime.Size = new Size(80, 24);
            dtpRunTime.TabIndex = 12;
            // 
            // btnSaveSchedule
            // 
            btnSaveSchedule.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveSchedule.BackColor = Color.FromArgb(30, 58, 138);
            btnSaveSchedule.Cursor = Cursors.Hand;
            btnSaveSchedule.FlatAppearance.BorderSize = 0;
            btnSaveSchedule.FlatStyle = FlatStyle.Flat;
            btnSaveSchedule.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSaveSchedule.ForeColor = Color.White;
            btnSaveSchedule.Location = new Point(870, 14);
            btnSaveSchedule.Name = "btnSaveSchedule";
            btnSaveSchedule.Size = new Size(150, 45);
            btnSaveSchedule.TabIndex = 13;
            btnSaveSchedule.Text = "Save Schedule";
            btnSaveSchedule.UseVisualStyleBackColor = false;
            btnSaveSchedule.Click += BtnSaveSchedule_Click;
            // 
            // lblNextRunStatus
            // 
            lblNextRunStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNextRunStatus.ForeColor = Color.FromArgb(30, 58, 138);
            lblNextRunStatus.Location = new Point(360, 72);
            lblNextRunStatus.Name = "lblNextRunStatus";
            lblNextRunStatus.Size = new Size(650, 22);
            lblNextRunStatus.TabIndex = 14;
            lblNextRunStatus.Text = "🕒 Next Run: Calculating...";
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 8.5F);
            lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblStatus.Location = new Point(20, 72);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(320, 22);
            lblStatus.TabIndex = 15;
            // 
            // pnlGrid
            // 
            pnlGrid.BackColor = Color.FromArgb(245, 247, 250);
            pnlGrid.Controls.Add(dgvResults);
            pnlGrid.Controls.Add(pnlGridHeader);
            pnlGrid.Dock = DockStyle.Fill;
            pnlGrid.Location = new Point(0, 214);
            pnlGrid.Name = "pnlGrid";
            pnlGrid.Padding = new Padding(12, 0, 12, 12);
            pnlGrid.Size = new Size(1034, 427);
            pnlGrid.TabIndex = 0;
            // 
            // dgvResults
            // 
            dgvResults.AllowUserToAddRows = false;
            dgvResults.AllowUserToDeleteRows = false;
            dgvResults.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(241, 245, 249);
            dgvResults.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResults.BackgroundColor = Color.White;
            dgvResults.BorderStyle = BorderStyle.None;
            dgvResults.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(30, 58, 138);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.Padding = new Padding(8, 0, 0, 0);
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvResults.ColumnHeadersHeight = 38;
            dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.5F);
            dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.Padding = new Padding(8, 2, 4, 2);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(191, 219, 254);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvResults.DefaultCellStyle = dataGridViewCellStyle3;
            dgvResults.Dock = DockStyle.Fill;
            dgvResults.EnableHeadersVisualStyles = false;
            dgvResults.Font = new Font("Segoe UI", 9.5F);
            dgvResults.GridColor = Color.FromArgb(226, 232, 240);
            dgvResults.Location = new Point(12, 36);
            dgvResults.MultiSelect = false;
            dgvResults.Name = "dgvResults";
            dgvResults.ReadOnly = true;
            dgvResults.RowHeadersVisible = false;
            dgvResults.RowTemplate.Height = 30;
            dgvResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResults.Size = new Size(1010, 379);
            dgvResults.TabIndex = 0;
            // 
            // pnlGridHeader
            // 
            pnlGridHeader.BackColor = Color.FromArgb(245, 247, 250);
            pnlGridHeader.Controls.Add(lblGridTitle);
            pnlGridHeader.Dock = DockStyle.Top;
            pnlGridHeader.Location = new Point(12, 0);
            pnlGridHeader.Name = "pnlGridHeader";
            pnlGridHeader.Size = new Size(1010, 36);
            pnlGridHeader.TabIndex = 1;
            // 
            // lblGridTitle
            // 
            lblGridTitle.Dock = DockStyle.Fill;
            lblGridTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblGridTitle.ForeColor = Color.FromArgb(30, 58, 138);
            lblGridTitle.Location = new Point(0, 0);
            lblGridTitle.Name = "lblGridTitle";
            lblGridTitle.Size = new Size(1010, 36);
            lblGridTitle.TabIndex = 0;
            lblGridTitle.Text = "EOD Results (Automated Service Operations)";
            lblGridTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlLogs
            // 
            pnlLogs.BackColor = Color.FromArgb(15, 23, 42);
            pnlLogs.Controls.Add(rtbLogs);
            pnlLogs.Dock = DockStyle.Bottom;
            pnlLogs.Location = new Point(0, 641);
            pnlLogs.Name = "pnlLogs";
            pnlLogs.Padding = new Padding(12);
            pnlLogs.Size = new Size(1034, 140);
            pnlLogs.TabIndex = 3;
            // 
            // rtbLogs
            // 
            rtbLogs.BackColor = Color.FromArgb(15, 23, 42);
            rtbLogs.BorderStyle = BorderStyle.None;
            rtbLogs.Dock = DockStyle.Fill;
            rtbLogs.Font = new Font("Consolas", 9F);
            rtbLogs.ForeColor = Color.FromArgb(200, 210, 220);
            rtbLogs.Location = new Point(12, 12);
            rtbLogs.Name = "rtbLogs";
            rtbLogs.ReadOnly = true;
            rtbLogs.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbLogs.Size = new Size(1010, 116);
            rtbLogs.TabIndex = 0;
            rtbLogs.Text = "Ready.\n";
            // 
            // mnuMain
            // 
            mnuMain.BackColor = Color.FromArgb(23, 48, 107);
            mnuMain.ForeColor = Color.White;
            mnuMain.Items.AddRange(new ToolStripItem[] { mnuItemSettings, mnuItemHistory });
            mnuMain.Location = new Point(0, 0);
            mnuMain.Name = "mnuMain";
            mnuMain.Size = new Size(1034, 24);
            mnuMain.TabIndex = 4;
            // 
            // mnuItemSettings
            // 
            mnuItemSettings.DropDownItems.AddRange(new ToolStripItem[] { mnuItemProviderSettings, mnuItemSymbolSettings, mnuItemDatabaseSettings, mnuItemAddStock });
            mnuItemSettings.ForeColor = Color.White;
            mnuItemSettings.Name = "mnuItemSettings";
            mnuItemSettings.Size = new Size(76, 20);
            mnuItemSettings.Text = "⚙ Settings";
            // 
            // mnuItemProviderSettings
            // 
            mnuItemProviderSettings.Name = "mnuItemProviderSettings";
            mnuItemProviderSettings.Size = new Size(255, 22);
            mnuItemProviderSettings.Text = "📡 Provider Settings";
            mnuItemProviderSettings.Click += MnuItemProviderSettings_Click;
            // 
            // mnuItemSymbolSettings
            // 
            mnuItemSymbolSettings.Name = "mnuItemSymbolSettings";
            mnuItemSymbolSettings.Size = new Size(255, 22);
            mnuItemSymbolSettings.Text = "🏷 Symbol Settings (EOD_STOCKS)";
            mnuItemSymbolSettings.Click += MnuItemSymbolSettings_Click;
            // 
            // mnuItemDatabaseSettings
            // 
            mnuItemDatabaseSettings.Name = "mnuItemDatabaseSettings";
            mnuItemDatabaseSettings.Size = new Size(255, 22);
            mnuItemDatabaseSettings.Text = "🗄 Database Connection";
            mnuItemDatabaseSettings.Click += MnuItemDatabaseSettings_Click;
            // 
            // mnuItemAddStock
            // 
            mnuItemAddStock.Name = "mnuItemAddStock";
            mnuItemAddStock.Size = new Size(255, 22);
            mnuItemAddStock.Text = "➕ Add Stock";
            mnuItemAddStock.Click += MnuItemAddStock_Click;
            // 
            // mnuItemHistory
            // 
            mnuItemHistory.ForeColor = Color.White;
            mnuItemHistory.Name = "mnuItemHistory";
            mnuItemHistory.Size = new Size(72, 20);
            mnuItemHistory.Text = "📋 History";
            mnuItemHistory.Click += MnuItemHistoricalData_Click;
            // 
            // tsToolBar
            // 
            tsToolBar.BackColor = Color.FromArgb(30, 58, 138);
            tsToolBar.Dock = DockStyle.None;
            tsToolBar.GripStyle = ToolStripGripStyle.Hidden;
            tsToolBar.Items.AddRange(new ToolStripItem[] { tsBtnRunNow });
            tsToolBar.Location = new Point(800, 0);
            tsToolBar.Name = "tsToolBar";
            tsToolBar.RenderMode = ToolStripRenderMode.System;
            tsToolBar.Size = new Size(106, 25);
            tsToolBar.TabIndex = 5;
            // 
            // tsBtnRunNow
            // 
            tsBtnRunNow.BackColor = Color.FromArgb(16, 185, 129);
            tsBtnRunNow.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsBtnRunNow.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            tsBtnRunNow.ForeColor = Color.White;
            tsBtnRunNow.Name = "tsBtnRunNow";
            tsBtnRunNow.Size = new Size(103, 22);
            tsBtnRunNow.Text = "▶ Run EOD Now";
            tsBtnRunNow.ToolTipText = "Manually trigger an immediate EOD data import";
            tsBtnRunNow.Click += TsBtnRunNow_Click;
            // 
            // SettingsForm
            // 
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1034, 781);
            Controls.Add(pnlGrid);
            Controls.Add(pnlLogs);
            Controls.Add(pnlControls);
            Controls.Add(pnlHeader);
            Controls.Add(mnuMain);
            Controls.Add(tsToolBar);
            Font = new Font("Segoe UI", 9.5F);
            MainMenuStrip = mnuMain;
            MinimumSize = new Size(800, 650);
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EOD Data Service — Settings & Schedule";
            pnlHeader.ResumeLayout(false);
            pnlControls.ResumeLayout(false);
            pnlControls.PerformLayout();
            pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvResults).EndInit();
            pnlGridHeader.ResumeLayout(false);
            pnlLogs.ResumeLayout(false);
            mnuMain.ResumeLayout(false);
            mnuMain.PerformLayout();
            tsToolBar.ResumeLayout(false);
            tsToolBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
