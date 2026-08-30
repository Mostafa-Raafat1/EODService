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
        private Label lblReuterId;
        private TextBox txtReuterId;
        private CheckBox chkReuterActive;

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
            lblReuterId = new Label();
            txtReuterId = new TextBox();
            chkReuterActive = new CheckBox();
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
            pnlHeader.Size = new Size(860, 70);
            pnlHeader.TabIndex = 0;

            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(700, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Stock & Symbol Manager (Oracle EOD_STOCKS)";

            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubtitle.Location = new Point(22, 42);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(700, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Select a stock to update parameters, active provider flags (Yahoo, Twelve Data, Reuters/LSEG), or delete from database";

            // ── pnlMain ───────────────────────────────────────────────────────────
            pnlMain.BackColor = Color.FromArgb(248, 250, 252);
            pnlMain.Controls.Add(lblCurrentSymbols);
            pnlMain.Controls.Add(dgvStocks);
            pnlMain.Controls.Add(grpEditStock);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 70);
            pnlMain.Name = "pnlMain";
            pnlMain.Padding = new Padding(16);
            pnlMain.Size = new Size(860, 480);
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
            dgvStocks.Size = new Size(826, 210);
            dgvStocks.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            grpEditStock.Controls.Add(lblReuterId);
            grpEditStock.Controls.Add(txtReuterId);
            grpEditStock.Controls.Add(chkReuterActive);
            grpEditStock.Controls.Add(btnUpdateStock);
            grpEditStock.Controls.Add(btnRemoveSymbol);
            grpEditStock.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grpEditStock.ForeColor = Color.FromArgb(30, 58, 138);
            grpEditStock.Location = new Point(16, 252);
            grpEditStock.Name = "grpEditStock";
            grpEditStock.Size = new Size(826, 215);
            grpEditStock.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpEditStock.TabIndex = 2;
            grpEditStock.TabStop = false;
            grpEditStock.Text = "Edit Selected Stock Details";

            // Stock ID (ReadOnly)
            lblStockId.AutoSize = true;
            lblStockId.ForeColor = Color.FromArgb(51, 65, 85);
            lblStockId.Location = new Point(16, 24);
            lblStockId.Name = "lblStockId";
            lblStockId.Text = "Stock ID:";

            txtStockId.BorderStyle = BorderStyle.FixedSingle;
            txtStockId.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtStockId.Location = new Point(16, 42);
            txtStockId.Name = "txtStockId";
            txtStockId.ReadOnly = true;
            txtStockId.Size = new Size(55, 23);
            txtStockId.TabIndex = 0;
            txtStockId.TextAlign = HorizontalAlignment.Center;
            txtStockId.BackColor = Color.FromArgb(241, 245, 249);

            // Company ID
            lblCompId.AutoSize = true;
            lblCompId.ForeColor = Color.FromArgb(51, 65, 85);
            lblCompId.Location = new Point(79, 24);
            lblCompId.Name = "lblCompId";
            lblCompId.Text = "Comp ID:";

            txtCompId.BorderStyle = BorderStyle.FixedSingle;
            txtCompId.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtCompId.Location = new Point(79, 42);
            txtCompId.Name = "txtCompId";
            txtCompId.Size = new Size(70, 23);
            txtCompId.TabIndex = 1;
            txtCompId.TextAlign = HorizontalAlignment.Center;

            // Stock Name
            lblStockName.AutoSize = true;
            lblStockName.ForeColor = Color.FromArgb(51, 65, 85);
            lblStockName.Location = new Point(157, 24);
            lblStockName.Name = "lblStockName";
            lblStockName.Text = "Stock Name:";

            txtStockName.BorderStyle = BorderStyle.FixedSingle;
            txtStockName.Font = new Font("Segoe UI", 9F);
            txtStockName.Location = new Point(157, 42);
            txtStockName.Name = "txtStockName";
            txtStockName.Size = new Size(270, 23);
            txtStockName.TabIndex = 2;

            // ISIN Code
            lblIsin.AutoSize = true;
            lblIsin.ForeColor = Color.FromArgb(51, 65, 85);
            lblIsin.Location = new Point(435, 24);
            lblIsin.Name = "lblIsin";
            lblIsin.Text = "ISIN Code:";

            txtIsin.BorderStyle = BorderStyle.FixedSingle;
            txtIsin.Font = new Font("Segoe UI", 9F);
            txtIsin.Location = new Point(435, 42);
            txtIsin.Name = "txtIsin";
            txtIsin.Size = new Size(170, 23);
            txtIsin.TabIndex = 3;

            // Exchange
            lblExchange.AutoSize = true;
            lblExchange.ForeColor = Color.FromArgb(51, 65, 85);
            lblExchange.Location = new Point(613, 24);
            lblExchange.Name = "lblExchange";
            lblExchange.Text = "Exchange:";

            txtExchange.BorderStyle = BorderStyle.FixedSingle;
            txtExchange.Font = new Font("Segoe UI", 9F);
            txtExchange.Location = new Point(613, 42);
            txtExchange.Name = "txtExchange";
            txtExchange.Size = new Size(195, 23);
            txtExchange.TabIndex = 4;

            // Row 2: 3 Providers side by side
            // 1. Yahoo Finance
            lblYahooId.AutoSize = true;
            lblYahooId.ForeColor = Color.FromArgb(51, 65, 85);
            lblYahooId.Location = new Point(16, 76);
            lblYahooId.Name = "lblYahooId";
            lblYahooId.Text = "Yahoo Finance Ticker:";

            txtYahooId.BorderStyle = BorderStyle.FixedSingle;
            txtYahooId.Font = new Font("Segoe UI", 9F);
            txtYahooId.Location = new Point(16, 94);
            txtYahooId.Name = "txtYahooId";
            txtYahooId.Size = new Size(145, 23);
            txtYahooId.TabIndex = 5;

            chkYahooActive.AutoSize = true;
            chkYahooActive.Checked = true;
            chkYahooActive.ForeColor = Color.FromArgb(51, 65, 85);
            chkYahooActive.Location = new Point(167, 96);
            chkYahooActive.Name = "chkYahooActive";
            chkYahooActive.Text = "YF Active";
            chkYahooActive.TabIndex = 6;
            chkYahooActive.CheckedChanged += ChkYahooActive_CheckedChanged;

            // 2. Twelve Data
            lblTwelveDataId.AutoSize = true;
            lblTwelveDataId.ForeColor = Color.FromArgb(51, 65, 85);
            lblTwelveDataId.Location = new Point(285, 76);
            lblTwelveDataId.Name = "lblTwelveDataId";
            lblTwelveDataId.Text = "Twelve Data Ticker:";

            txtTwelveDataId.BorderStyle = BorderStyle.FixedSingle;
            txtTwelveDataId.Font = new Font("Segoe UI", 9F);
            txtTwelveDataId.Location = new Point(285, 94);
            txtTwelveDataId.Name = "txtTwelveDataId";
            txtTwelveDataId.Size = new Size(145, 23);
            txtTwelveDataId.TabIndex = 7;

            chkTwelveDataActive.AutoSize = true;
            chkTwelveDataActive.Checked = true;
            chkTwelveDataActive.ForeColor = Color.FromArgb(51, 65, 85);
            chkTwelveDataActive.Location = new Point(436, 96);
            chkTwelveDataActive.Name = "chkTwelveDataActive";
            chkTwelveDataActive.Text = "TD Active";
            chkTwelveDataActive.TabIndex = 8;
            chkTwelveDataActive.CheckedChanged += ChkTwelveDataActive_CheckedChanged;

            // 3. Reuters / LSEG
            lblReuterId.AutoSize = true;
            lblReuterId.ForeColor = Color.FromArgb(51, 65, 85);
            lblReuterId.Location = new Point(555, 76);
            lblReuterId.Name = "lblReuterId";
            lblReuterId.Text = "Reuters / LSEG Ticker:";

            txtReuterId.BorderStyle = BorderStyle.FixedSingle;
            txtReuterId.Font = new Font("Segoe UI", 9F);
            txtReuterId.Location = new Point(555, 94);
            txtReuterId.Name = "txtReuterId";
            txtReuterId.Size = new Size(145, 23);
            txtReuterId.TabIndex = 9;

            chkReuterActive.AutoSize = true;
            chkReuterActive.Checked = true;
            chkReuterActive.ForeColor = Color.FromArgb(51, 65, 85);
            chkReuterActive.Location = new Point(706, 96);
            chkReuterActive.Name = "chkReuterActive";
            chkReuterActive.Text = "LSEG Active";
            chkReuterActive.TabIndex = 10;
            chkReuterActive.CheckedChanged += ChkReuterActive_CheckedChanged;

            // Action Buttons inside Card
            btnUpdateStock.BackColor = Color.FromArgb(16, 185, 129); // Emerald Green
            btnUpdateStock.Cursor = Cursors.Hand;
            btnUpdateStock.FlatAppearance.BorderSize = 0;
            btnUpdateStock.FlatStyle = FlatStyle.Flat;
            btnUpdateStock.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnUpdateStock.ForeColor = Color.White;
            btnUpdateStock.Location = new Point(16, 150);
            btnUpdateStock.Name = "btnUpdateStock";
            btnUpdateStock.Size = new Size(160, 36);
            btnUpdateStock.TabIndex = 11;
            btnUpdateStock.Text = "💾 Apply Updates";
            btnUpdateStock.UseVisualStyleBackColor = false;
            btnUpdateStock.Click += BtnUpdateStock_Click;

            btnRemoveSymbol.BackColor = Color.FromArgb(225, 29, 72); // Rose Red
            btnRemoveSymbol.Cursor = Cursors.Hand;
            btnRemoveSymbol.FlatAppearance.BorderSize = 0;
            btnRemoveSymbol.FlatStyle = FlatStyle.Flat;
            btnRemoveSymbol.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRemoveSymbol.ForeColor = Color.White;
            btnRemoveSymbol.Location = new Point(190, 150);
            btnRemoveSymbol.Name = "btnRemoveSymbol";
            btnRemoveSymbol.Size = new Size(160, 36);
            btnRemoveSymbol.TabIndex = 12;
            btnRemoveSymbol.Text = "🗑 Delete Stock";
            btnRemoveSymbol.UseVisualStyleBackColor = false;
            btnRemoveSymbol.Click += BtnRemoveSymbol_Click;

            // ── pnlFooter ─────────────────────────────────────────────────────────
            pnlFooter.BackColor = Color.FromArgb(226, 232, 240);
            pnlFooter.Controls.Add(btnSaveSymbolSettings);
            pnlFooter.Controls.Add(lblSymbolSettingsStatus);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 550);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(860, 60);
            pnlFooter.TabIndex = 2;

            btnSaveSymbolSettings.BackColor = Color.FromArgb(30, 58, 138);
            btnSaveSymbolSettings.Cursor = Cursors.Hand;
            btnSaveSymbolSettings.FlatAppearance.BorderSize = 0;
            btnSaveSymbolSettings.FlatStyle = FlatStyle.Flat;
            btnSaveSymbolSettings.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSaveSymbolSettings.ForeColor = Color.White;
            btnSaveSymbolSettings.Location = new Point(610, 12);
            btnSaveSymbolSettings.Name = "btnSaveSymbolSettings";
            btnSaveSymbolSettings.Size = new Size(232, 36);
            btnSaveSymbolSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveSymbolSettings.TabIndex = 0;
            btnSaveSymbolSettings.Text = "💾 Save Database Changes";
            btnSaveSymbolSettings.UseVisualStyleBackColor = false;
            btnSaveSymbolSettings.Click += BtnSaveSymbolSettings_Click;

            lblSymbolSettingsStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblSymbolSettingsStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblSymbolSettingsStatus.Location = new Point(14, 20);
            lblSymbolSettingsStatus.Name = "lblSymbolSettingsStatus";
            lblSymbolSettingsStatus.Size = new Size(580, 22);
            lblSymbolSettingsStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblSymbolSettingsStatus.TabIndex = 1;

            // ── SymbolSettingsForm ────────────────────────────────────────────────
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(860, 610);
            Controls.Add(pnlMain);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(876, 649);
            MinimumSize = new Size(876, 649);
            Name = "SymbolSettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "TICKR";

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
