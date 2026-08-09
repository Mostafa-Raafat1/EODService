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
        private Button       btnGetData;
        private Label        lblStatus;
        private Panel        pnlGrid;       // outer wrapper (DockStyle.Fill)
        private Panel        pnlGridHeader; // top strip inside pnlGrid
        private Label        lblGridTitle;
        private DataGridView dgvResults;    // fills the rest of pnlGrid
        
        private Panel        pnlLogs;
        private RichTextBox  rtbLogs;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlControls = new Panel();
            lblProviderLabel = new Label();
            cmbProvider = new ComboBox();
            lblProviderHint = new Label();
            btnGetData = new Button();
            lblStatus = new Label();
            pnlGrid = new Panel();
            dgvResults = new DataGridView();
            pnlGridHeader = new Panel();
            lblGridTitle = new Label();
            pnlLogs = new Panel();
            rtbLogs = new RichTextBox();
            pnlHeader.SuspendLayout();
            pnlControls.SuspendLayout();
            pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvResults).BeginInit();
            pnlGridHeader.SuspendLayout();
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
            pnlHeader.Size = new Size(1034, 80);
            pnlHeader.TabIndex = 2;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(600, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "EOD Data Service";
            // 
            // lblSubtitle
            // 
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubtitle.Location = new Point(22, 48);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(700, 22);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Select a provider and click Get Data to fetch and save EOD prices";
            // 
            // pnlControls
            // 
            pnlControls.BackColor = Color.FromArgb(235, 238, 245);
            pnlControls.Controls.Add(lblProviderLabel);
            pnlControls.Controls.Add(cmbProvider);
            pnlControls.Controls.Add(lblProviderHint);
            pnlControls.Controls.Add(btnGetData);
            pnlControls.Controls.Add(lblStatus);
            pnlControls.Dock = DockStyle.Top;
            pnlControls.Location = new Point(0, 80);
            pnlControls.Name = "pnlControls";
            pnlControls.Size = new Size(1034, 90);
            pnlControls.TabIndex = 1;
            // 
            // lblProviderLabel
            // 
            lblProviderLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblProviderLabel.ForeColor = Color.FromArgb(30, 41, 59);
            lblProviderLabel.Location = new Point(20, 18);
            lblProviderLabel.Name = "lblProviderLabel";
            lblProviderLabel.Size = new Size(160, 22);
            lblProviderLabel.TabIndex = 0;
            lblProviderLabel.Text = "Active Data Provider:";
            // 
            // cmbProvider
            // 
            cmbProvider.BackColor = Color.White;
            cmbProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProvider.FlatStyle = FlatStyle.Flat;
            cmbProvider.Font = new Font("Segoe UI", 10F);
            cmbProvider.ForeColor = Color.FromArgb(15, 23, 42);
            cmbProvider.Items.AddRange(new object[] { "TwelveData", "Yahoo" });
            cmbProvider.Location = new Point(190, 14);
            cmbProvider.Name = "cmbProvider";
            cmbProvider.Size = new Size(200, 25);
            cmbProvider.TabIndex = 0;
            // 
            // lblProviderHint
            // 
            lblProviderHint.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblProviderHint.ForeColor = Color.FromArgb(100, 116, 139);
            lblProviderHint.Location = new Point(20, 52);
            lblProviderHint.Name = "lblProviderHint";
            lblProviderHint.Size = new Size(500, 18);
            lblProviderHint.TabIndex = 1;
            lblProviderHint.Text = "Data will be fetched and saved to Oracle DB automatically.";
            // 
            // btnGetData
            // 
            btnGetData.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGetData.BackColor = Color.FromArgb(30, 58, 138);
            btnGetData.Cursor = Cursors.Hand;
            btnGetData.FlatAppearance.BorderSize = 0;
            btnGetData.FlatStyle = FlatStyle.Flat;
            btnGetData.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGetData.ForeColor = Color.White;
            btnGetData.Location = new Point(874, 24);
            btnGetData.Name = "btnGetData";
            btnGetData.Size = new Size(140, 40);
            btnGetData.TabIndex = 1;
            btnGetData.Text = "Get Data";
            btnGetData.UseVisualStyleBackColor = false;
            btnGetData.Click += BtnGetData_Click;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblStatus.Location = new Point(400, 32);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(460, 22);
            lblStatus.TabIndex = 2;
            // 
            // pnlGrid
            // 
            pnlGrid.BackColor = Color.FromArgb(245, 247, 250);
            pnlGrid.Controls.Add(dgvResults);
            pnlGrid.Controls.Add(pnlGridHeader);
            pnlGrid.Dock = DockStyle.Fill;
            pnlGrid.Location = new Point(0, 170);
            pnlGrid.Name = "pnlGrid";
            pnlGrid.Padding = new Padding(12, 0, 12, 12);
            pnlGrid.Size = new Size(1034, 471);
            pnlGrid.TabIndex = 0;
            // 
            // dgvResults
            // 
            dgvResults.AllowUserToAddRows = false;
            dgvResults.AllowUserToDeleteRows = false;
            dgvResults.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(241, 245, 249);
            dgvResults.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResults.BackgroundColor = Color.White;
            dgvResults.BorderStyle = BorderStyle.None;
            dgvResults.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(30, 58, 138);
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = Color.White;
            dataGridViewCellStyle5.Padding = new Padding(8, 0, 0, 0);
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvResults.ColumnHeadersHeight = 38;
            dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = SystemColors.Window;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9.5F);
            dataGridViewCellStyle6.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle6.Padding = new Padding(8, 2, 4, 2);
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(191, 219, 254);
            dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvResults.DefaultCellStyle = dataGridViewCellStyle6;
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
            dgvResults.Size = new Size(1010, 423);
            dgvResults.TabIndex = 0;
            dgvResults.CellContentClick += dgvResults_CellContentClick_1;
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
            lblGridTitle.Text = "EOD Results";
            lblGridTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblGridTitle.Click += lblGridTitle_Click;
            // 
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
            rtbLogs.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            rtbLogs.ForeColor = Color.FromArgb(200, 210, 220);
            rtbLogs.Location = new Point(12, 12);
            rtbLogs.Name = "rtbLogs";
            rtbLogs.ReadOnly = true;
            rtbLogs.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbLogs.Size = new Size(1010, 116);
            rtbLogs.TabIndex = 0;
            rtbLogs.Text = "Ready.\n";
            // 
            // SettingsForm
            // 
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1034, 781);
            Controls.Add(pnlGrid);
            Controls.Add(pnlLogs);
            Controls.Add(pnlControls);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            MinimumSize = new Size(800, 650);
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EOD Data Service — Settings & Results";
            pnlHeader.ResumeLayout(false);
            pnlControls.ResumeLayout(false);
            pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvResults).EndInit();
            pnlGridHeader.ResumeLayout(false);
            pnlLogs.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
