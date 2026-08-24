using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class ProviderSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;

        // Tabbed provider fields (fill between header and footer)
        private TabControl tabProviders;
        private TabPage    tabPageYahoo;
        private TabPage    tabPageTwelveData;
        private TabPage    tabPageReuters;

        // Yahoo tab
        private Label   lblYahooBaseUrl;
        private TextBox txtYahooBaseUrl;
        private Label   lblYahooEndpoint;
        private TextBox txtYahooEndpoint;
        private Label   lblYahooApiKey;
        private TextBox txtYahooApiKey;
        private Label   lblYahooParameters;
        private TextBox txtYahooParameters;

        // TwelveData tab
        private Label   lblTwelveBaseUrl;
        private TextBox txtTwelveBaseUrl;
        private Label   lblTwelveEndpoint;
        private TextBox txtTwelveEndpoint;
        private Label   lblTwelveApiKey;
        private TextBox txtTwelveApiKey;
        private Label   lblTwelveParameters;
        private TextBox txtTwelveParameters;

        // Reuters tab
        private Label   lblReutersBaseUrl;
        private TextBox txtReutersBaseUrl;
        private Label   lblReutersEndpoint;
        private TextBox txtReutersEndpoint;
        private Label   lblReutersApiKey;
        private TextBox txtReutersApiKey;
        private Label   lblReutersParameters;
        private TextBox txtReutersParameters;

        // Footer
        private Panel  pnlFooter;
        private Button btnSaveProviderSettings;
        private Label  lblProviderSettingsStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader                 = new Panel();
            lblTitle                  = new Label();
            lblSubtitle               = new Label();
            tabProviders              = new TabControl();
            tabPageYahoo              = new TabPage();
            tabPageTwelveData         = new TabPage();
            tabPageReuters            = new TabPage();
            lblYahooBaseUrl           = new Label();
            txtYahooBaseUrl           = new TextBox();
            lblYahooEndpoint          = new Label();
            txtYahooEndpoint          = new TextBox();
            lblYahooApiKey            = new Label();
            txtYahooApiKey            = new TextBox();
            lblYahooParameters        = new Label();
            txtYahooParameters        = new TextBox();
            lblTwelveBaseUrl          = new Label();
            txtTwelveBaseUrl          = new TextBox();
            lblTwelveEndpoint         = new Label();
            txtTwelveEndpoint         = new TextBox();
            lblTwelveApiKey           = new Label();
            txtTwelveApiKey           = new TextBox();
            lblTwelveParameters       = new Label();
            txtTwelveParameters       = new TextBox();
            lblReutersBaseUrl         = new Label();
            txtReutersBaseUrl         = new TextBox();
            lblReutersEndpoint        = new Label();
            txtReutersEndpoint        = new TextBox();
            lblReutersApiKey          = new Label();
            txtReutersApiKey          = new TextBox();
            lblReutersParameters      = new Label();
            txtReutersParameters      = new TextBox();
            pnlFooter                 = new Panel();
            btnSaveProviderSettings   = new Button();
            lblProviderSettingsStatus = new Label();

            SuspendLayout();
            pnlHeader.SuspendLayout();
            tabProviders.SuspendLayout();
            tabPageYahoo.SuspendLayout();
            tabPageTwelveData.SuspendLayout();
            tabPageReuters.SuspendLayout();
            pnlFooter.SuspendLayout();

            // ── pnlHeader ─────────────────────────────────────────────────────────
            pnlHeader.BackColor = Color.FromArgb(30, 58, 138);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock     = DockStyle.Top;
            pnlHeader.Name     = "pnlHeader";
            pnlHeader.Size     = new Size(460, 70);
            pnlHeader.TabIndex = 0;

            lblTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location  = new Point(20, 10);
            lblTitle.Name      = "lblTitle";
            lblTitle.Size      = new Size(380, 28);
            lblTitle.TabIndex  = 0;
            lblTitle.Text      = "Provider Settings";

            lblSubtitle.Font      = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubtitle.Location  = new Point(22, 42);
            lblSubtitle.Name      = "lblSubtitle";
            lblSubtitle.Size      = new Size(380, 20);
            lblSubtitle.TabIndex  = 1;
            lblSubtitle.Text      = "Edit Yahoo, TwelveData, and Reuters settings and JSON parameters";

            // ── tabProviders (fill between header and footer) ─────────────────────
            tabProviders.Controls.Add(tabPageYahoo);
            tabProviders.Controls.Add(tabPageTwelveData);
            tabProviders.Controls.Add(tabPageReuters);
            tabProviders.Dock          = DockStyle.Fill;
            tabProviders.Font          = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tabProviders.ItemSize      = new Size(110, 26);
            tabProviders.Name          = "tabProviders";
            tabProviders.SelectedIndex = 0;
            tabProviders.Size          = new Size(460, 350);
            tabProviders.TabIndex      = 1;

            // ── tabPageYahoo ──────────────────────────────────────────────────────
            tabPageYahoo.BackColor = Color.FromArgb(248, 250, 252);
            tabPageYahoo.Controls.Add(lblYahooBaseUrl);
            tabPageYahoo.Controls.Add(txtYahooBaseUrl);
            tabPageYahoo.Controls.Add(lblYahooEndpoint);
            tabPageYahoo.Controls.Add(txtYahooEndpoint);
            tabPageYahoo.Controls.Add(lblYahooApiKey);
            tabPageYahoo.Controls.Add(txtYahooApiKey);
            tabPageYahoo.Controls.Add(lblYahooParameters);
            tabPageYahoo.Controls.Add(txtYahooParameters);
            tabPageYahoo.Name    = "tabPageYahoo";
            tabPageYahoo.Padding = new Padding(3);
            tabPageYahoo.Text    = "  Yahoo Finance  ";

            ConfigureFieldLabel(lblYahooBaseUrl,    "lblYahooBaseUrl",    "Base URL",          14,  14);
            ConfigureFieldBox  (txtYahooBaseUrl,     "txtYahooBaseUrl",                         14,  34);
            ConfigureFieldLabel(lblYahooEndpoint,   "lblYahooEndpoint",   "Endpoint",          14,  64);
            ConfigureFieldBox  (txtYahooEndpoint,   "txtYahooEndpoint",                        14,  84);
            ConfigureFieldLabel(lblYahooApiKey,     "lblYahooApiKey",     "API Key (Optional)", 14, 114, 140);
            ConfigureFieldBox  (txtYahooApiKey,     "txtYahooApiKey",                          14, 134);
            ConfigureFieldLabel(lblYahooParameters, "lblYahooParameters", "Parameters (JSON)", 14, 164, 200);
            ConfigureJsonBox   (txtYahooParameters, "txtYahooParameters",                      14, 186);

            // ── tabPageTwelveData ─────────────────────────────────────────────────
            tabPageTwelveData.BackColor = Color.FromArgb(248, 250, 252);
            tabPageTwelveData.Controls.Add(lblTwelveBaseUrl);
            tabPageTwelveData.Controls.Add(txtTwelveBaseUrl);
            tabPageTwelveData.Controls.Add(lblTwelveEndpoint);
            tabPageTwelveData.Controls.Add(txtTwelveEndpoint);
            tabPageTwelveData.Controls.Add(lblTwelveApiKey);
            tabPageTwelveData.Controls.Add(txtTwelveApiKey);
            tabPageTwelveData.Controls.Add(lblTwelveParameters);
            tabPageTwelveData.Controls.Add(txtTwelveParameters);
            tabPageTwelveData.Name    = "tabPageTwelveData";
            tabPageTwelveData.Padding = new Padding(3);
            tabPageTwelveData.Text    = "  TwelveData  ";

            ConfigureFieldLabel(lblTwelveBaseUrl,    "lblTwelveBaseUrl",    "Base URL",          14,  14);
            ConfigureFieldBox  (txtTwelveBaseUrl,    "txtTwelveBaseUrl",                         14,  34);
            ConfigureFieldLabel(lblTwelveEndpoint,   "lblTwelveEndpoint",   "Endpoint",          14,  64);
            ConfigureFieldBox  (txtTwelveEndpoint,   "txtTwelveEndpoint",                        14,  84);
            ConfigureFieldLabel(lblTwelveApiKey,     "lblTwelveApiKey",     "API Key",           14, 114);
            ConfigureFieldBox  (txtTwelveApiKey,     "txtTwelveApiKey",                          14, 134);
            ConfigureFieldLabel(lblTwelveParameters, "lblTwelveParameters", "Parameters (JSON)", 14, 164, 200);
            ConfigureJsonBox   (txtTwelveParameters, "txtTwelveParameters",                      14, 186);

            // ── tabPageReuters ───────────────────────────────────────────────────
            tabPageReuters.BackColor = Color.FromArgb(248, 250, 252);
            tabPageReuters.Controls.Add(lblReutersBaseUrl);
            tabPageReuters.Controls.Add(txtReutersBaseUrl);
            tabPageReuters.Controls.Add(lblReutersEndpoint);
            tabPageReuters.Controls.Add(txtReutersEndpoint);
            tabPageReuters.Controls.Add(lblReutersApiKey);
            tabPageReuters.Controls.Add(txtReutersApiKey);
            tabPageReuters.Controls.Add(lblReutersParameters);
            tabPageReuters.Controls.Add(txtReutersParameters);
            tabPageReuters.Name    = "tabPageReuters";
            tabPageReuters.Padding = new Padding(3);
            tabPageReuters.Text    = "  Reuters (LSEG)  ";

            ConfigureFieldLabel(lblReutersBaseUrl,    "lblReutersBaseUrl",    "Base URL (ws://...)", 14,  14, 150);
            ConfigureFieldBox  (txtReutersBaseUrl,    "txtReutersBaseUrl",                           14,  34);
            ConfigureFieldLabel(lblReutersEndpoint,   "lblReutersEndpoint",   "Endpoint",            14,  64);
            ConfigureFieldBox  (txtReutersEndpoint,   "txtReutersEndpoint",                          14,  84);
            ConfigureFieldLabel(lblReutersApiKey,     "lblReutersApiKey",     "API Key (Optional)",   14, 114, 140);
            ConfigureFieldBox  (txtReutersApiKey,     "txtReutersApiKey",                            14, 134);
            ConfigureFieldLabel(lblReutersParameters, "lblReutersParameters", "Parameters (JSON)",   14, 164, 200);
            ConfigureJsonBox   (txtReutersParameters, "txtReutersParameters",                        14, 186);

            // ── pnlFooter ─────────────────────────────────────────────────────────
            pnlFooter.BackColor = Color.FromArgb(226, 232, 240);
            pnlFooter.Controls.Add(btnSaveProviderSettings);
            pnlFooter.Controls.Add(lblProviderSettingsStatus);
            pnlFooter.Dock      = DockStyle.Bottom;
            pnlFooter.Name      = "pnlFooter";
            pnlFooter.Size      = new Size(460, 60);
            pnlFooter.TabIndex  = 2;

            btnSaveProviderSettings.BackColor                 = Color.FromArgb(30, 58, 138);
            btnSaveProviderSettings.Cursor                    = Cursors.Hand;
            btnSaveProviderSettings.FlatAppearance.BorderSize = 0;
            btnSaveProviderSettings.FlatStyle                 = FlatStyle.Flat;
            btnSaveProviderSettings.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSaveProviderSettings.ForeColor                 = Color.White;
            btnSaveProviderSettings.Location                  = new Point(268, 12);
            btnSaveProviderSettings.Name                      = "btnSaveProviderSettings";
            btnSaveProviderSettings.Size                      = new Size(178, 36);
            btnSaveProviderSettings.TabIndex                  = 0;
            btnSaveProviderSettings.Text                      = "Save Provider Settings";
            btnSaveProviderSettings.UseVisualStyleBackColor   = false;
            btnSaveProviderSettings.Click                    += BtnSaveProviderSettings_Click;

            lblProviderSettingsStatus.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblProviderSettingsStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblProviderSettingsStatus.Location  = new Point(14, 20);
            lblProviderSettingsStatus.Name      = "lblProviderSettingsStatus";
            lblProviderSettingsStatus.Size      = new Size(248, 22);
            lblProviderSettingsStatus.TabIndex  = 1;

            // ── ProviderSettingsForm ──────────────────────────────────────────────
            // Controls.Add order: fill first (lowest Z), then footer, then header (highest Z).
            BackColor       = Color.FromArgb(245, 247, 250);
            ClientSize      = new Size(460, 480);
            Controls.Add(tabProviders);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            Font            = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            MaximumSize     = new Size(476, 519);
            MinimumSize     = new Size(476, 519);
            Name            = "ProviderSettingsForm";
            StartPosition   = FormStartPosition.CenterParent;
            Text            = "Provider Settings";

            tabPageYahoo.ResumeLayout(false);
            tabPageYahoo.PerformLayout();
            tabPageTwelveData.ResumeLayout(false);
            tabPageTwelveData.PerformLayout();
            tabProviders.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── Private layout helpers ────────────────────────────────────────────────

        /// <summary>Applies the uniform bold label style to every field caption.</summary>
        private static void ConfigureFieldLabel(
            Label label, string name, string text, int x, int y, int width = 100)
        {
            label.AutoSize  = false;
            label.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(51, 65, 85);
            label.Location  = new Point(x, y);
            label.Name      = name;
            label.Size      = new Size(width, 18);
            label.Text      = text;
        }

        /// <summary>Applies the uniform text-box style to single-line editable fields.</summary>
        private static void ConfigureFieldBox(
            TextBox box, string name, int x, int y)
        {
            box.BorderStyle = BorderStyle.FixedSingle;
            box.Font        = new Font("Segoe UI", 9.5F);
            box.Location    = new Point(x, y);
            box.Name        = name;
            box.Size        = new Size(424, 24);
        }

        /// <summary>Applies the code-editor text-box style to multiline JSON fields.</summary>
        private static void ConfigureJsonBox(
            TextBox box, string name, int x, int y, int width = 424, int height = 135)
        {
            box.BorderStyle = BorderStyle.FixedSingle;
            box.Font        = new Font("Consolas", 9.5F);
            box.Location    = new Point(x, y);
            box.Multiline   = true;
            box.Name        = name;
            box.ScrollBars  = ScrollBars.Vertical;
            box.Size        = new Size(width, height);
        }
    }
}
