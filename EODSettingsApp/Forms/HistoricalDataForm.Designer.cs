using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class HistoricalDataForm
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;

        // Filter Controls
        private Panel pnlFilter;
        private Label lblSymbol;
        private ComboBox cmbSymbol;
        private Label lblFromDate;
        private DateTimePicker dtpFromDate;
        private Label lblToDate;
        private DateTimePicker dtpToDate;
        private Button btnSearchHistory;

        // Stats bar — records count only
        private Panel pnlStats;
        private Label lblTotalRecords;

        // Main Grid
        private Panel pnlGrid;
        private DataGridView dgvHistory;

        // Footer
        private Panel pnlFooter;
        private Label lblHistoryStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            var headerStyle = new DataGridViewCellStyle();
            var cellStyle = new DataGridViewCellStyle();

            pnlHeader       = new Panel();
            lblTitle        = new Label();
            lblSubtitle     = new Label();
            pnlFilter       = new Panel();
            lblSymbol       = new Label();
            cmbSymbol       = new ComboBox();
            lblFromDate     = new Label();
            dtpFromDate     = new DateTimePicker();
            lblToDate       = new Label();
            dtpToDate       = new DateTimePicker();
            btnSearchHistory = new Button();
            pnlStats        = new Panel();
            lblTotalRecords = new Label();
            pnlGrid         = new Panel();
            dgvHistory      = new DataGridView();
            pnlFooter       = new Panel();
            lblHistoryStatus = new Label();

            pnlHeader.SuspendLayout();
            pnlFilter.SuspendLayout();
            pnlStats.SuspendLayout();
            pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            pnlFooter.SuspendLayout();
            SuspendLayout();

            // ── pnlHeader ─────────────────────────────────────────────────────────
            pnlHeader.BackColor = Color.FromArgb(30, 58, 138);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(840, 70);
            pnlHeader.TabIndex = 0;

            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(600, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Historical Data Explorer";

            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubtitle.Location = new Point(22, 42);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(600, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Query and inspect historical stock prices from Oracle database";

            // ── pnlFilter ─────────────────────────────────────────────────────────
            pnlFilter.BackColor = Color.FromArgb(241, 245, 249);
            pnlFilter.Controls.Add(lblSymbol);
            pnlFilter.Controls.Add(cmbSymbol);
            pnlFilter.Controls.Add(lblFromDate);
            pnlFilter.Controls.Add(dtpFromDate);
            pnlFilter.Controls.Add(lblToDate);
            pnlFilter.Controls.Add(dtpToDate);
            pnlFilter.Controls.Add(btnSearchHistory);
            pnlFilter.Dock = DockStyle.Top;
            pnlFilter.Name = "pnlFilter";
            pnlFilter.Padding = new Padding(16);
            pnlFilter.Size = new Size(840, 60);
            pnlFilter.TabIndex = 1;

            lblSymbol.AutoSize = true;
            lblSymbol.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblSymbol.ForeColor = Color.FromArgb(51, 65, 85);
            lblSymbol.Location = new Point(16, 22);
            lblSymbol.Name = "lblSymbol";
            lblSymbol.TabIndex = 0;
            lblSymbol.Text = "Symbol:";

            cmbSymbol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSymbol.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            cmbSymbol.FormattingEnabled = true;
            cmbSymbol.Location = new Point(72, 17);
            cmbSymbol.Name = "cmbSymbol";
            cmbSymbol.Size = new Size(110, 25);
            cmbSymbol.TabIndex = 1;

            lblFromDate.AutoSize = true;
            lblFromDate.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblFromDate.ForeColor = Color.FromArgb(51, 65, 85);
            lblFromDate.Location = new Point(195, 22);
            lblFromDate.Name = "lblFromDate";
            lblFromDate.TabIndex = 2;
            lblFromDate.Text = "From:";

            dtpFromDate.CustomFormat = "yyyy-MM-dd";
            dtpFromDate.Format = DateTimePickerFormat.Custom;
            dtpFromDate.Font = new Font("Segoe UI", 9.5F);
            dtpFromDate.Location = new Point(238, 17);
            dtpFromDate.Name = "dtpFromDate";
            dtpFromDate.Size = new Size(115, 24);
            dtpFromDate.TabIndex = 3;

            lblToDate.AutoSize = true;
            lblToDate.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblToDate.ForeColor = Color.FromArgb(51, 65, 85);
            lblToDate.Location = new Point(362, 22);
            lblToDate.Name = "lblToDate";
            lblToDate.TabIndex = 4;
            lblToDate.Text = "To:";

            dtpToDate.CustomFormat = "yyyy-MM-dd";
            dtpToDate.Format = DateTimePickerFormat.Custom;
            dtpToDate.Font = new Font("Segoe UI", 9.5F);
            dtpToDate.Location = new Point(390, 17);
            dtpToDate.Name = "dtpToDate";
            dtpToDate.Size = new Size(115, 24);
            dtpToDate.TabIndex = 5;

            btnSearchHistory.BackColor = Color.FromArgb(30, 58, 138);
            btnSearchHistory.Cursor = Cursors.Hand;
            btnSearchHistory.FlatAppearance.BorderSize = 0;
            btnSearchHistory.FlatStyle = FlatStyle.Flat;
            btnSearchHistory.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSearchHistory.ForeColor = Color.White;
            btnSearchHistory.Location = new Point(520, 16);
            btnSearchHistory.Name = "btnSearchHistory";
            btnSearchHistory.Size = new Size(130, 27);
            btnSearchHistory.TabIndex = 6;
            btnSearchHistory.Text = "🔍 Search History";
            btnSearchHistory.UseVisualStyleBackColor = false;
            btnSearchHistory.Click += BtnSearchHistory_Click;

            // ── pnlStats (records count only) ─────────────────────────────────────
            pnlStats.BackColor = Color.FromArgb(226, 232, 240);
            pnlStats.Controls.Add(lblTotalRecords);
            pnlStats.Dock = DockStyle.Top;
            pnlStats.Name = "pnlStats";
            pnlStats.Padding = new Padding(12);
            pnlStats.Size = new Size(840, 34);
            pnlStats.TabIndex = 2;

            lblTotalRecords.AutoSize = true;
            lblTotalRecords.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblTotalRecords.ForeColor = Color.FromArgb(30, 58, 138);
            lblTotalRecords.Location = new Point(16, 9);
            lblTotalRecords.Name = "lblTotalRecords";
            lblTotalRecords.TabIndex = 0;
            lblTotalRecords.Text = "Records: 0";

            // ── pnlGrid ───────────────────────────────────────────────────────────
            pnlGrid.Controls.Add(dgvHistory);
            pnlGrid.Dock = DockStyle.Fill;
            pnlGrid.Name = "pnlGrid";
            pnlGrid.Padding = new Padding(12);
            pnlGrid.TabIndex = 3;

            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AllowUserToDeleteRows = false;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.BackgroundColor = Color.White;
            dgvHistory.BorderStyle = BorderStyle.Fixed3D;

            headerStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            headerStyle.BackColor = Color.FromArgb(23, 48, 107);
            headerStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            headerStyle.ForeColor = Color.White;
            headerStyle.SelectionBackColor = Color.FromArgb(23, 48, 107);
            headerStyle.SelectionForeColor = Color.White;
            headerStyle.WrapMode = DataGridViewTriState.True;
            dgvHistory.ColumnHeadersDefaultCellStyle = headerStyle;
            dgvHistory.ColumnHeadersHeight = 30;

            cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            cellStyle.BackColor = Color.White;
            cellStyle.Font = new Font("Segoe UI", 9F);
            cellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            cellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
            cellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
            cellStyle.WrapMode = DataGridViewTriState.False;
            dgvHistory.DefaultCellStyle = cellStyle;

            dgvHistory.Dock = DockStyle.Fill;
            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.Name = "dgvHistory";
            dgvHistory.ReadOnly = true;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.TabIndex = 0;

            // ── pnlFooter ─────────────────────────────────────────────────────────
            pnlFooter.BackColor = Color.FromArgb(226, 232, 240);
            pnlFooter.Controls.Add(lblHistoryStatus);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(840, 40);
            pnlFooter.TabIndex = 4;

            lblHistoryStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblHistoryStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblHistoryStatus.Location = new Point(16, 11);
            lblHistoryStatus.Name = "lblHistoryStatus";
            lblHistoryStatus.Size = new Size(800, 20);
            lblHistoryStatus.TabIndex = 0;
            lblHistoryStatus.Text = "Ready to search historical records.";

            // ── HistoricalDataForm ────────────────────────────────────────────────
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(840, 540);
            Controls.Add(pnlGrid);
            Controls.Add(pnlStats);
            Controls.Add(pnlFooter);
            Controls.Add(pnlFilter);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(856, 579);
            Name = "HistoricalDataForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Historical Data Explorer";

            pnlHeader.ResumeLayout(false);
            pnlFilter.ResumeLayout(false);
            pnlFilter.PerformLayout();
            pnlStats.ResumeLayout(false);
            pnlStats.PerformLayout();
            pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
