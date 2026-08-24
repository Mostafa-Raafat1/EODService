using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class AddStockForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Header ───────────────────────────────────────────────────────────
        private Panel  pnlHeader;
        private Label  lblTitle;

        // ── Main content ─────────────────────────────────────────────────────
        private Panel  pnlMain;

        private Label   lblStockNameLabel;
        private TextBox txtStockName;

        private Label   lblInitialIdLabel;
        private TextBox txtInitialId;

        private Label   lblExchangeLabel;
        private TextBox txtExchange;

        // TD Tradable & TD Symbol
        private Label       lblTdTradable;
        private GroupBox    grpTdTradable;
        private RadioButton rdoTdYes;
        private RadioButton rdoTdNo;
        private Label       lblTdSymbolLabel;
        private TextBox     txtTdSymbol;

        // YF Tradable & YF Symbol
        private Label       lblYfTradable;
        private GroupBox    grpYfTradable;
        private RadioButton rdoYfYes;
        private RadioButton rdoYfNo;
        private Label       lblYfSymbolLabel;
        private TextBox     txtYfSymbol;

        // LSEG Tradable & LSEG Symbol
        private Label       lblLsegTradable;
        private GroupBox    grpLsegTradable;
        private RadioButton rdoLsegYes;
        private RadioButton rdoLsegNo;
        private Label       lblLsegSymbolLabel;
        private TextBox     txtLsegSymbol;
        // ISIN
        private Label       lblIsinLabel;
        private TextBox     txtIsin;

        // ── Footer ───────────────────────────────────────────────────────────
        private Panel  pnlFooter;
        private Button btnAdd;
        private Button btnClear;
        private Label  lblStatus;

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

            pnlMain = new Panel();
            lblStockNameLabel = new Label();
            txtStockName = new TextBox();
            lblInitialIdLabel = new Label();
            txtInitialId = new TextBox();

            lblExchangeLabel = new Label();
            txtExchange = new TextBox();

            lblTdTradable = new Label();
            grpTdTradable = new GroupBox();
            rdoTdYes = new RadioButton();
            rdoTdNo = new RadioButton();
            lblTdSymbolLabel = new Label();
            txtTdSymbol = new TextBox();

            lblYfTradable = new Label();
            grpYfTradable = new GroupBox();
            rdoYfYes = new RadioButton();
            rdoYfNo = new RadioButton();
            lblYfSymbolLabel = new Label();
            txtYfSymbol = new TextBox();

            lblLsegTradable = new Label();
            grpLsegTradable = new GroupBox();
            rdoLsegYes = new RadioButton();
            rdoLsegNo = new RadioButton();
            lblLsegSymbolLabel = new Label();
            txtLsegSymbol = new TextBox();
            lblIsinLabel = new Label();
            txtIsin = new TextBox();

            pnlFooter = new Panel();
            btnAdd = new Button();
            btnClear = new Button();
            lblStatus = new Label();

            pnlHeader.SuspendLayout();
            pnlMain.SuspendLayout();
            grpTdTradable.SuspendLayout();
            grpYfTradable.SuspendLayout();
            grpLsegTradable.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();

            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(30, 58, 138);
            pnlHeader.Controls.Add(lblTitle);
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
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Add New Stock";

            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.FromArgb(248, 250, 252);
            pnlMain.Controls.Add(lblStockNameLabel);
            pnlMain.Controls.Add(txtStockName);
            pnlMain.Controls.Add(lblInitialIdLabel);
            pnlMain.Controls.Add(txtInitialId);
            pnlMain.Controls.Add(lblExchangeLabel);
            pnlMain.Controls.Add(txtExchange);
            pnlMain.Controls.Add(lblTdTradable);
            pnlMain.Controls.Add(grpTdTradable);
            pnlMain.Controls.Add(lblTdSymbolLabel);
            pnlMain.Controls.Add(txtTdSymbol);
            pnlMain.Controls.Add(lblYfTradable);
            pnlMain.Controls.Add(grpYfTradable);
            pnlMain.Controls.Add(lblYfSymbolLabel);
            pnlMain.Controls.Add(txtYfSymbol);
            pnlMain.Controls.Add(lblLsegTradable);
            pnlMain.Controls.Add(grpLsegTradable);
            pnlMain.Controls.Add(lblLsegSymbolLabel);
            pnlMain.Controls.Add(txtLsegSymbol);            pnlMain.Controls.Add(lblIsinLabel);
            pnlMain.Controls.Add(txtIsin);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 70);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(460, 390);
            pnlMain.TabIndex = 1;

            // 
            // Row 1: Stock Name (left) | Initial ID (right)
            // 
            lblStockNameLabel.AutoSize = true;
            lblStockNameLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblStockNameLabel.ForeColor = Color.FromArgb(51, 65, 85);
            lblStockNameLabel.Location = new Point(20, 18);
            lblStockNameLabel.Name = "lblStockNameLabel";
            lblStockNameLabel.Size = new Size(82, 15);
            lblStockNameLabel.TabIndex = 0;
            lblStockNameLabel.Text = "Stock Name *";

            txtStockName.BorderStyle = BorderStyle.FixedSingle;
            txtStockName.Font = new Font("Segoe UI", 9.5F);
            txtStockName.Location = new Point(20, 36);
            txtStockName.Name = "txtStockName";
            txtStockName.Size = new Size(190, 24);
            txtStockName.TabIndex = 1;

            lblInitialIdLabel.AutoSize = true;
            lblInitialIdLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblInitialIdLabel.ForeColor = Color.FromArgb(51, 65, 85);
            lblInitialIdLabel.Location = new Point(240, 18);
            lblInitialIdLabel.Name = "lblInitialIdLabel";
            lblInitialIdLabel.Size = new Size(54, 15);
            lblInitialIdLabel.TabIndex = 2;
            lblInitialIdLabel.Text = "Initial ID";

            txtInitialId.BorderStyle = BorderStyle.FixedSingle;
            txtInitialId.Font = new Font("Segoe UI", 9.5F);
            txtInitialId.Location = new Point(240, 36);
            txtInitialId.Name = "txtInitialId";
            txtInitialId.Size = new Size(200, 24);
            txtInitialId.TabIndex = 3;

            // 
            // Row 2: Exchange
            // 
            lblExchangeLabel.AutoSize = true;
            lblExchangeLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblExchangeLabel.ForeColor = Color.FromArgb(51, 65, 85);
            lblExchangeLabel.Location = new Point(20, 76);
            lblExchangeLabel.Name = "lblExchangeLabel";
            lblExchangeLabel.Size = new Size(60, 15);
            lblExchangeLabel.TabIndex = 4;
            lblExchangeLabel.Text = "Exchange";

            txtExchange.BorderStyle = BorderStyle.FixedSingle;
            txtExchange.Font = new Font("Segoe UI", 9.5F);
            txtExchange.Location = new Point(20, 94);
            txtExchange.Name = "txtExchange";
            txtExchange.Size = new Size(420, 24);
            txtExchange.TabIndex = 5;

            // 
            // Row 3: Yahoo Finance
            // 
            lblTdTradable.AutoSize = true;
            lblTdTradable.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblTdTradable.ForeColor = Color.FromArgb(51, 65, 85);
            lblTdTradable.Location = new Point(240, 194);
            lblTdTradable.Name = "lblTdTradable";
            lblTdTradable.Size = new Size(73, 15);
            lblTdTradable.TabIndex = 6;
            lblTdTradable.Text = "TD Tradable";

            grpTdTradable.BackColor = Color.FromArgb(248, 250, 252);
            grpTdTradable.Controls.Add(rdoTdYes);
            grpTdTradable.Controls.Add(rdoTdNo);
            grpTdTradable.FlatStyle = FlatStyle.Flat;
            grpTdTradable.Font = new Font("Segoe UI", 8.5F);
            grpTdTradable.ForeColor = Color.FromArgb(100, 116, 139);
            grpTdTradable.Location = new Point(236, 210);
            grpTdTradable.Name = "grpTdTradable";
            grpTdTradable.Size = new Size(200, 44);
            grpTdTradable.TabIndex = 7;
            grpTdTradable.TabStop = false;

            rdoTdYes.AutoSize = true;
            rdoTdYes.Checked = true;
            rdoTdYes.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            rdoTdYes.ForeColor = Color.FromArgb(22, 163, 74);
            rdoTdYes.Location = new Point(10, 12);
            rdoTdYes.Name = "rdoTdYes";
            rdoTdYes.Size = new Size(46, 21);
            rdoTdYes.TabIndex = 0;
            rdoTdYes.TabStop = true;
            rdoTdYes.Text = "Yes";
            rdoTdYes.UseVisualStyleBackColor = true;

            rdoTdNo.AutoSize = true;
            rdoTdNo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            rdoTdNo.ForeColor = Color.FromArgb(185, 28, 28);
            rdoTdNo.Location = new Point(90, 12);
            rdoTdNo.Name = "rdoTdNo";
            rdoTdNo.Size = new Size(44, 21);
            rdoTdNo.TabIndex = 1;
            rdoTdNo.Text = "No";
            rdoTdNo.UseVisualStyleBackColor = true;

            lblYfTradable.AutoSize = true;
            lblYfTradable.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblYfTradable.ForeColor = Color.FromArgb(51, 65, 85);
            lblYfTradable.Location = new Point(240, 134);
            lblYfTradable.Name = "lblYfTradable";
            lblYfTradable.Size = new Size(70, 15);
            lblYfTradable.TabIndex = 8;
            lblYfTradable.Text = "YF Tradable";

            grpYfTradable.BackColor = Color.FromArgb(248, 250, 252);
            grpYfTradable.Controls.Add(rdoYfYes);
            grpYfTradable.Controls.Add(rdoYfNo);
            grpYfTradable.FlatStyle = FlatStyle.Flat;
            grpYfTradable.Font = new Font("Segoe UI", 8.5F);
            grpYfTradable.ForeColor = Color.FromArgb(100, 116, 139);
            grpYfTradable.Location = new Point(236, 150);
            grpYfTradable.Name = "grpYfTradable";
            grpYfTradable.Size = new Size(200, 44);
            grpYfTradable.TabIndex = 9;
            grpYfTradable.TabStop = false;

            rdoYfYes.AutoSize = true;
            rdoYfYes.Checked = true;
            rdoYfYes.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            rdoYfYes.ForeColor = Color.FromArgb(22, 163, 74);
            rdoYfYes.Location = new Point(10, 12);
            rdoYfYes.Name = "rdoYfYes";
            rdoYfYes.Size = new Size(46, 21);
            rdoYfYes.TabIndex = 0;
            rdoYfYes.TabStop = true;
            rdoYfYes.Text = "Yes";
            rdoYfYes.UseVisualStyleBackColor = true;

            rdoYfNo.AutoSize = true;
            rdoYfNo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            rdoYfNo.ForeColor = Color.FromArgb(185, 28, 28);
            rdoYfNo.Location = new Point(90, 12);
            rdoYfNo.Name = "rdoYfNo";
            rdoYfNo.Size = new Size(44, 21);
            rdoYfNo.TabIndex = 1;
            rdoYfNo.Text = "No";
            rdoYfNo.UseVisualStyleBackColor = true;

            // 
            // Row 4: Twelve Data
            // 
            lblTdSymbolLabel.AutoSize = true;
            lblTdSymbolLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblTdSymbolLabel.ForeColor = Color.FromArgb(51, 65, 85);
            lblTdSymbolLabel.Location = new Point(20, 194);
            lblTdSymbolLabel.Name = "lblTdSymbolLabel";
            lblTdSymbolLabel.Size = new Size(67, 15);
            lblTdSymbolLabel.TabIndex = 10;
            lblTdSymbolLabel.Text = "TD Symbol";

            txtTdSymbol.BorderStyle = BorderStyle.FixedSingle;
            txtTdSymbol.Font = new Font("Segoe UI", 9.5F);
            txtTdSymbol.Location = new Point(20, 212);
            txtTdSymbol.Name = "txtTdSymbol";
            txtTdSymbol.Size = new Size(200, 24);
            txtTdSymbol.TabIndex = 11;

            lblYfSymbolLabel.AutoSize = true;
            lblYfSymbolLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblYfSymbolLabel.ForeColor = Color.FromArgb(51, 65, 85);
            lblYfSymbolLabel.Location = new Point(20, 134);
            lblYfSymbolLabel.Name = "lblYfSymbolLabel";
            lblYfSymbolLabel.Size = new Size(64, 15);
            lblYfSymbolLabel.TabIndex = 12;
            lblYfSymbolLabel.Text = "YF Symbol";

            txtYfSymbol.BorderStyle = BorderStyle.FixedSingle;
            txtYfSymbol.Font = new Font("Segoe UI", 9.5F);
            txtYfSymbol.Location = new Point(20, 152);
            txtYfSymbol.Name = "txtYfSymbol";
            txtYfSymbol.Size = new Size(200, 24);
            txtYfSymbol.TabIndex = 13;

            // 
            // Row 5: LSEG
            // 
            lblLsegTradable.AutoSize = true;
            lblLsegTradable.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblLsegTradable.ForeColor = Color.FromArgb(51, 65, 85);
            lblLsegTradable.Location = new Point(240, 254);
            lblLsegTradable.Name = "lblLsegTradable";
            lblLsegTradable.Size = new Size(89, 15);
            lblLsegTradable.TabIndex = 14;
            lblLsegTradable.Text = "LSEG Tradable";

            grpLsegTradable.BackColor = Color.FromArgb(248, 250, 252);
            grpLsegTradable.Controls.Add(rdoLsegYes);
            grpLsegTradable.Controls.Add(rdoLsegNo);
            grpLsegTradable.FlatStyle = FlatStyle.Flat;
            grpLsegTradable.Font = new Font("Segoe UI", 8.5F);
            grpLsegTradable.ForeColor = Color.FromArgb(100, 116, 139);
            grpLsegTradable.Location = new Point(236, 270);
            grpLsegTradable.Name = "grpLsegTradable";
            grpLsegTradable.Size = new Size(200, 44);
            grpLsegTradable.TabIndex = 15;
            grpLsegTradable.TabStop = false;

            rdoLsegYes.AutoSize = true;
            rdoLsegYes.Checked = true;
            rdoLsegYes.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            rdoLsegYes.ForeColor = Color.FromArgb(22, 163, 74);
            rdoLsegYes.Location = new Point(10, 12);
            rdoLsegYes.Name = "rdoLsegYes";
            rdoLsegYes.Size = new Size(46, 21);
            rdoLsegYes.TabIndex = 0;
            rdoLsegYes.TabStop = true;
            rdoLsegYes.Text = "Yes";
            rdoLsegYes.UseVisualStyleBackColor = true;

            rdoLsegNo.AutoSize = true;
            rdoLsegNo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            rdoLsegNo.ForeColor = Color.FromArgb(185, 28, 28);
            rdoLsegNo.Location = new Point(90, 12);
            rdoLsegNo.Name = "rdoLsegNo";
            rdoLsegNo.Size = new Size(44, 21);
            rdoLsegNo.TabIndex = 1;
            rdoLsegNo.Text = "No";
            rdoLsegNo.UseVisualStyleBackColor = true;

            lblLsegSymbolLabel.AutoSize = true;
            lblLsegSymbolLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblLsegSymbolLabel.ForeColor = Color.FromArgb(51, 65, 85);
            lblLsegSymbolLabel.Location = new Point(20, 254);
            lblLsegSymbolLabel.Name = "lblLsegSymbolLabel";
            lblLsegSymbolLabel.Size = new Size(80, 15);
            lblLsegSymbolLabel.TabIndex = 16;
            lblLsegSymbolLabel.Text = "LSEG Symbol";

            txtLsegSymbol.BorderStyle = BorderStyle.FixedSingle;
            txtLsegSymbol.Font = new Font("Segoe UI", 9.5F);
            txtLsegSymbol.Location = new Point(20, 272);
            txtLsegSymbol.Name = "txtLsegSymbol";
            txtLsegSymbol.Size = new Size(200, 24);
            txtLsegSymbol.TabIndex = 17;
            // `r`n            // Row 6: ISIN
            // 
            lblIsinLabel.AutoSize = true;
            lblIsinLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblIsinLabel.ForeColor = Color.FromArgb(51, 65, 85);
            lblIsinLabel.Location = new Point(20, 314);
            lblIsinLabel.Name = "lblIsinLabel";
            lblIsinLabel.Size = new Size(33, 15);
            lblIsinLabel.TabIndex = 18;
            lblIsinLabel.Text = "ISIN";

            txtIsin.BorderStyle = BorderStyle.FixedSingle;
            txtIsin.Font = new Font("Segoe UI", 9.5F);
            txtIsin.Location = new Point(20, 332);
            txtIsin.Name = "txtIsin";
            txtIsin.Size = new Size(420, 24);
            txtIsin.TabIndex = 19;

            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.FromArgb(226, 232, 240);
            pnlFooter.Controls.Add(btnAdd);
            pnlFooter.Controls.Add(btnClear);
            pnlFooter.Controls.Add(lblStatus);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 460);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(460, 60);
            pnlFooter.TabIndex = 2;

            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.FromArgb(30, 58, 138);
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(270, 12);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(100, 36);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Add Stock";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += BtnAdd_Click;

            // 
            // btnClear
            // 
            btnClear.BackColor = Color.FromArgb(100, 116, 139);
            btnClear.Cursor = Cursors.Hand;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnClear.ForeColor = Color.White;
            btnClear.Location = new Point(380, 12);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(70, 36);
            btnClear.TabIndex = 1;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += BtnClear_Click;

            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblStatus.Location = new Point(14, 10);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(248, 40);
            lblStatus.AutoEllipsis = true;
            lblStatus.TabIndex = 2;

            // 
            // stockadd
            // 
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(460, 520);
            Controls.Add(pnlMain);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(476, 559);
            MinimumSize = new Size(476, 559);
            Name = "stockadd";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add New Stock";
            pnlHeader.ResumeLayout(false);
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            grpTdTradable.ResumeLayout(false);
            grpTdTradable.PerformLayout();
            grpYfTradable.ResumeLayout(false);
            grpYfTradable.PerformLayout();
            grpLsegTradable.ResumeLayout(false);
            grpLsegTradable.PerformLayout();
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}

