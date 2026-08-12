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

        // Main Panel & Grid
        private Panel pnlMain;
        private Label lblCurrentSymbols;
        private DataGridView dgvStocks;

        // Input / Edit Controls
        private GroupBox grpEditStock;
        private Label lblStockId;
        private TextBox txtStockId;
        private Label lblCompId;
        private TextBox txtCompId;
        private Label lblStockName;
        private TextBox txtStockName;
        private Label lblIsin;
        private TextBox txtIsin;
        private Label lblExchange;
        private TextBox txtExchange;
        private Label lblYahooId;
        private TextBox txtYahooId;
        private CheckBox chkYahooActive;
        private Label lblTwelveDataId;
        private TextBox txtTwelveDataId;
        private CheckBox chkTwelveDataActive;

        // Buttons
        private Button btnUpdateStock;
        private Button btnRemoveSymbol;

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
            var headerStyle = new DataGridViewCellStyle();
            var cellStyle = new DataGridViewCellStyle();

            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlMain = new Panel();
            lblCurrentSymbols = new Label();
            dgvStocks = new DataGridView();
            grpEditStock = new GroupBox();
            lblStockId = new Label();
            txtStockId = new TextBox();
            lblCompId = new Label();
            txtCompId = new TextBox();
            lblStockName = new Label();
            txtStockName = new TextBox();
            lblIsin = new Label();
            txtIsin = new TextBox();
            lblExchange = new Label();
            txtExchange = new TextBox();
            lblYahooId = new Label();
            txtYahooId = new TextBox();
            chkYahooActive = new CheckBox();
            lblTwelveDataId = new Label();
            txtTwelveDataId = new TextBox();
            chkTwelveDataActive = new CheckBox();
            btnUpdateStock = new Button();
            btnRemoveSymbol = new Button();
            pnlFooter = new Panel();
            btnSaveSymbolSettings = new Button();
            lblSymbolSettingsStatus = new Label();

            pnlHeader.SuspendLayout();
            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStocks).BeginInit();
            grpEditStock.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();

            // ── pnlHeader ─────────────────────────────────────────────────────────
            pnlHeader.BackColor = Color.FromArgb(30, 58, 138);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(760, 70);
            pnlHeader.TabIndex = 0;

            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(600, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Stock & Symbol Manager (Oracle EOD_STOCKS)";

            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubtitle.Location = new Point(22, 42);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(600, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Select a stock to update parameters, active provider flags, or delete from database";

            // ── pnlMain ───────────────────────────────────────────────────────────
            pnlMain.BackColor = Color.FromArgb(248, 250, 252);
            pnlMain.Controls.Add(lblCurrentSymbols);
            pnlMain.Controls.Add(dgvStocks);
            pnlMain.Controls.Add(grpEditStock);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 70);
            pnlMain.Name = "pnlMain";
            pnlMain.Padding = new Padding(16);
            pnlMain.Size = new Size(760, 410);
            pnlMain.TabIndex = 1;

            lblCurrentSymbols.AutoSize = true;
            lblCurrentSymbols.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblCurrentSymbols.ForeColor = Color.FromArgb(51, 65, 85);
            lblCurrentSymbols.Location = new Point(16, 12);
            lblCurrentSymbols.Name = "lblCurrentSymbols";
            lblCurrentSymbols.Size = new Size(224, 15);
            lblCurrentSymbols.TabIndex = 0;
            lblCurrentSymbols.Text = "Registered Stocks (Select a row to edit):";

            // DataGridView
            dgvStocks.AllowUserToAddRows = false;
            dgvStocks.AllowUserToDeleteRows = false;
            dgvStocks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStocks.BackgroundColor = Color.White;
            dgvStocks.BorderStyle = BorderStyle.Fixed3D;

            headerStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            headerStyle.BackColor = Color.FromArgb(23, 48, 107);
            headerStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            headerStyle.ForeColor = Color.White;
            headerStyle.SelectionBackColor = Color.FromArgb(23, 48, 107);
            headerStyle.SelectionForeColor = Color.White;
            headerStyle.WrapMode = DataGridViewTriState.True;
            dgvStocks.ColumnHeadersDefaultCellStyle = headerStyle;
            dgvStocks.ColumnHeadersHeight = 28;

            cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            cellStyle.BackColor = Color.White;
            cellStyle.Font = new Font("Segoe UI", 8.5F);
            cellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            cellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
            cellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
            cellStyle.WrapMode = DataGridViewTriState.False;
            dgvStocks.DefaultCellStyle = cellStyle;

            dgvStocks.EnableHeadersVisualStyles = false;
            dgvStocks.Location = new Point(16, 32);
            dgvStocks.Name = "dgvStocks";
            dgvStocks.ReadOnly = true;
            dgvStocks.RowHeadersVisible = false;
            dgvStocks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStocks.Size = new Size(726, 175);
            dgvStocks.TabIndex = 1;
            dgvStocks.SelectionChanged += DgvStocks_SelectionChanged;

            // GroupBox Edit
            grpEditStock.Controls.Add(lblStockId);
            grpEditStock.Controls.Add(txtStockId);
            grpEditStock.Controls.Add(lblCompId);
            grpEditStock.Controls.Add(txtCompId);
            grpEditStock.Controls.Add(lblStockName);
            grpEditStock.Controls.Add(txtStockName);
            grpEditStock.Controls.Add(lblIsin);
            grpEditStock.Controls.Add(txtIsin);
            grpEditStock.Controls.Add(lblExchange);
            grpEditStock.Controls.Add(txtExchange);
            grpEditStock.Controls.Add(lblYahooId);
            grpEditStock.Controls.Add(txtYahooId);
            grpEditStock.Controls.Add(chkYahooActive);
            grpEditStock.Controls.Add(lblTwelveDataId);
            grpEditStock.Controls.Add(txtTwelveDataId);
            grpEditStock.Controls.Add(chkTwelveDataActive);
            grpEditStock.Controls.Add(btnUpdateStock);
            grpEditStock.Controls.Add(btnRemoveSymbol);
            grpEditStock.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grpEditStock.ForeColor = Color.FromArgb(30, 58, 138);
            grpEditStock.Location = new Point(16, 215);
            grpEditStock.Name = "grpEditStock";
            grpEditStock.Size = new Size(726, 185);
            grpEditStock.TabIndex = 2;
            grpEditStock.TabStop = false;
            grpEditStock.Text = "Edit Selected Stock Details";

            // Stock ID (ReadOnly)
            lblStockId.AutoSize = true;
            lblStockId.ForeColor = Color.FromArgb(51, 65, 85);
            lblStockId.Location = new Point(16, 26);
            lblStockId.Name = "lblStockId";
            lblStockId.Text = "Stock ID:";

            txtStockId.BorderStyle = BorderStyle.FixedSingle;
            txtStockId.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtStockId.Location = new Point(16, 44);
            txtStockId.Name = "txtStockId";
            txtStockId.ReadOnly = true;
            txtStockId.Size = new Size(50, 23);
            txtStockId.TabIndex = 0;
            txtStockId.TextAlign = HorizontalAlignment.Center;
            txtStockId.BackColor = Color.FromArgb(241, 245, 249);

            // Company ID
            lblCompId.AutoSize = true;
            lblCompId.ForeColor = Color.FromArgb(51, 65, 85);
            lblCompId.Location = new Point(74, 26);
            lblCompId.Name = "lblCompId";
            lblCompId.Text = "Comp ID:";

            txtCompId.BorderStyle = BorderStyle.FixedSingle;
            txtCompId.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtCompId.Location = new Point(74, 44);
            txtCompId.Name = "txtCompId";
            txtCompId.Size = new Size(65, 23);
            txtCompId.TabIndex = 1;
            txtCompId.TextAlign = HorizontalAlignment.Center;

            // Stock Name
            lblStockName.AutoSize = true;
            lblStockName.ForeColor = Color.FromArgb(51, 65, 85);
            lblStockName.Location = new Point(147, 26);
            lblStockName.Name = "lblStockName";
            lblStockName.Text = "Stock Name:";

            txtStockName.BorderStyle = BorderStyle.FixedSingle;
            txtStockName.Font = new Font("Segoe UI", 9F);
            txtStockName.Location = new Point(147, 44);
            txtStockName.Name = "txtStockName";
            txtStockName.Size = new Size(220, 23);
            txtStockName.TabIndex = 2;

            // ISIN Code
            lblIsin.AutoSize = true;
            lblIsin.ForeColor = Color.FromArgb(51, 65, 85);
            lblIsin.Location = new Point(375, 26);
            lblIsin.Name = "lblIsin";
            lblIsin.Text = "ISIN Code:";

            txtIsin.BorderStyle = BorderStyle.FixedSingle;
            txtIsin.Font = new Font("Segoe UI", 9F);
            txtIsin.Location = new Point(375, 44);
            txtIsin.Name = "txtIsin";
            txtIsin.Size = new Size(130, 23);
            txtIsin.TabIndex = 3;

            // Exchange
            lblExchange.AutoSize = true;
            lblExchange.ForeColor = Color.FromArgb(51, 65, 85);
            lblExchange.Location = new Point(513, 26);
            lblExchange.Name = "lblExchange";
            lblExchange.Text = "Exchange:";

            txtExchange.BorderStyle = BorderStyle.FixedSingle;
            txtExchange.Font = new Font("Segoe UI", 9F);
            txtExchange.Location = new Point(513, 44);
            txtExchange.Name = "txtExchange";
            txtExchange.Size = new Size(100, 23);
            txtExchange.TabIndex = 4;

            // Yahoo Ticker & Flag
            lblYahooId.AutoSize = true;
            lblYahooId.ForeColor = Color.FromArgb(51, 65, 85);
            lblYahooId.Location = new Point(16, 78);
            lblYahooId.Name = "lblYahooId";
            lblYahooId.Text = "Yahoo Finance Ticker:";

            txtYahooId.BorderStyle = BorderStyle.FixedSingle;
            txtYahooId.Font = new Font("Segoe UI", 9F);
            txtYahooId.Location = new Point(16, 96);
            txtYahooId.Name = "txtYahooId";
            txtYahooId.Size = new Size(200, 23);
            txtYahooId.TabIndex = 3;

            chkYahooActive.AutoSize = true;
            chkYahooActive.Checked = true;
            chkYahooActive.ForeColor = Color.FromArgb(51, 65, 85);
            chkYahooActive.Location = new Point(224, 98);
            chkYahooActive.Name = "chkYahooActive";
            chkYahooActive.Text = "Yahoo Active";
            chkYahooActive.TabIndex = 4;
            chkYahooActive.CheckedChanged += ChkYahooActive_CheckedChanged;

            // TwelveData Ticker & Flag
            lblTwelveDataId.AutoSize = true;
            lblTwelveDataId.ForeColor = Color.FromArgb(51, 65, 85);
            lblTwelveDataId.Location = new Point(340, 78);
            lblTwelveDataId.Name = "lblTwelveDataId";
            lblTwelveDataId.Text = "Twelve Data Ticker:";

            txtTwelveDataId.BorderStyle = BorderStyle.FixedSingle;
            txtTwelveDataId.Font = new Font("Segoe UI", 9F);
            txtTwelveDataId.Location = new Point(340, 96);
            txtTwelveDataId.Name = "txtTwelveDataId";
            txtTwelveDataId.Size = new Size(200, 23);
            txtTwelveDataId.TabIndex = 5;

            chkTwelveDataActive.AutoSize = true;
            chkTwelveDataActive.Checked = true;
            chkTwelveDataActive.ForeColor = Color.FromArgb(51, 65, 85);
            chkTwelveDataActive.Location = new Point(548, 98);
            chkTwelveDataActive.Name = "chkTwelveDataActive";
            chkTwelveDataActive.Text = "TD Active";
            chkTwelveDataActive.TabIndex = 6;
            chkTwelveDataActive.CheckedChanged += ChkTwelveDataActive_CheckedChanged;

            // Action Buttons inside Card
            btnUpdateStock.BackColor = Color.FromArgb(16, 185, 129); // Emerald Green
            btnUpdateStock.Cursor = Cursors.Hand;
            btnUpdateStock.FlatAppearance.BorderSize = 0;
            btnUpdateStock.FlatStyle = FlatStyle.Flat;
            btnUpdateStock.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnUpdateStock.ForeColor = Color.White;
            btnUpdateStock.Location = new Point(16, 134);
            btnUpdateStock.Name = "btnUpdateStock";
            btnUpdateStock.Size = new Size(160, 34);
            btnUpdateStock.TabIndex = 7;
            btnUpdateStock.Text = "💾 Apply Updates";
            btnUpdateStock.UseVisualStyleBackColor = false;
            btnUpdateStock.Click += BtnUpdateStock_Click;

            btnRemoveSymbol.BackColor = Color.FromArgb(225, 29, 72); // Rose Red
            btnRemoveSymbol.Cursor = Cursors.Hand;
            btnRemoveSymbol.FlatAppearance.BorderSize = 0;
            btnRemoveSymbol.FlatStyle = FlatStyle.Flat;
            btnRemoveSymbol.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRemoveSymbol.ForeColor = Color.White;
            btnRemoveSymbol.Location = new Point(190, 134);
            btnRemoveSymbol.Name = "btnRemoveSymbol";
            btnRemoveSymbol.Size = new Size(160, 34);
            btnRemoveSymbol.TabIndex = 8;
            btnRemoveSymbol.Text = "🗑 Delete Stock";
            btnRemoveSymbol.UseVisualStyleBackColor = false;
            btnRemoveSymbol.Click += BtnRemoveSymbol_Click;

            // ── pnlFooter ─────────────────────────────────────────────────────────
            pnlFooter.BackColor = Color.FromArgb(226, 232, 240);
            pnlFooter.Controls.Add(btnSaveSymbolSettings);
            pnlFooter.Controls.Add(lblSymbolSettingsStatus);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 480);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(760, 60);
            pnlFooter.TabIndex = 2;

            btnSaveSymbolSettings.BackColor = Color.FromArgb(30, 58, 138);
            btnSaveSymbolSettings.Cursor = Cursors.Hand;
            btnSaveSymbolSettings.FlatAppearance.BorderSize = 0;
            btnSaveSymbolSettings.FlatStyle = FlatStyle.Flat;
            btnSaveSymbolSettings.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSaveSymbolSettings.ForeColor = Color.White;
            btnSaveSymbolSettings.Location = new Point(520, 12);
            btnSaveSymbolSettings.Name = "btnSaveSymbolSettings";
            btnSaveSymbolSettings.Size = new Size(220, 36);
            btnSaveSymbolSettings.TabIndex = 0;
            btnSaveSymbolSettings.Text = "💾 Save Database Changes";
            btnSaveSymbolSettings.UseVisualStyleBackColor = false;
            btnSaveSymbolSettings.Click += BtnSaveSymbolSettings_Click;

            lblSymbolSettingsStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblSymbolSettingsStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblSymbolSettingsStatus.Location = new Point(14, 20);
            lblSymbolSettingsStatus.Name = "lblSymbolSettingsStatus";
            lblSymbolSettingsStatus.Size = new Size(490, 22);
            lblSymbolSettingsStatus.TabIndex = 1;

            // ── SymbolSettingsForm ────────────────────────────────────────────────
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(760, 540);
            Controls.Add(pnlMain);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(776, 579);
            MinimumSize = new Size(776, 579);
            Name = "SymbolSettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Stock & Symbol Manager (EOD_STOCKS)";

            pnlHeader.ResumeLayout(false);
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStocks).EndInit();
            grpEditStock.ResumeLayout(false);
            grpEditStock.PerformLayout();
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
