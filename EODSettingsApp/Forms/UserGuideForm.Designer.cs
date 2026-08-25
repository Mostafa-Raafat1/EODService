using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class UserGuideForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Shell Panels ─────────────────────────────────────────────────────────
        private Panel  pnlHeader;
        private Label  lblHeaderTitle;
        private Label  lblHeaderSubtitle;
        private Panel  pnlMain;            // Fill — left sidebar + right content
        private Panel  pnlSidebar;         // Left 215px — navigation
        private Panel  pnlContentWrapper;  // Fill — content header + scroll area
        private Panel  pnlContentHeader;   // Top 54px — icon + section title strip
        private Label  lblContentIcon;
        private Label  lblContentTitle;
        private Panel  pnlContentArea;     // Fill, AutoScroll — rendered section body
        private Panel  pnlFooter;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader         = new Panel();
            lblHeaderTitle    = new Label();
            lblHeaderSubtitle = new Label();
            pnlMain           = new Panel();
            pnlSidebar        = new Panel();
            pnlContentWrapper = new Panel();
            pnlContentHeader  = new Panel();
            lblContentIcon    = new Label();
            lblContentTitle   = new Label();
            pnlContentArea    = new Panel();
            pnlFooter         = new Panel();
            btnClose          = new Button();

            pnlHeader.SuspendLayout();
            pnlMain.SuspendLayout();
            pnlContentWrapper.SuspendLayout();
            pnlContentHeader.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();

            // ── pnlHeader ───────────────────────────────────────────────────────
            pnlHeader.BackColor = Color.FromArgb(23, 48, 107);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Dock    = DockStyle.Top;
            pnlHeader.Height  = 68;
            pnlHeader.Padding = new Padding(20, 0, 0, 0);

            lblHeaderTitle.AutoSize  = true;
            lblHeaderTitle.Font      = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location  = new Point(20, 10);
            lblHeaderTitle.Text      = "📖  EOD Service — User Guide";

            lblHeaderSubtitle.AutoSize  = true;
            lblHeaderSubtitle.Font      = new Font("Segoe UI", 9F);
            lblHeaderSubtitle.ForeColor = Color.FromArgb(155, 195, 255);
            lblHeaderSubtitle.Location  = new Point(23, 40);
            lblHeaderSubtitle.Text      = "Complete system administration, configuration, and troubleshooting reference";

            // ── pnlSidebar ──────────────────────────────────────────────────────
            pnlSidebar.AutoScroll = true;
            pnlSidebar.BackColor  = Color.FromArgb(15, 30, 80);
            pnlSidebar.Dock       = DockStyle.Left;
            pnlSidebar.Width      = 215;

            // ── pnlContentWrapper ───────────────────────────────────────────────
            pnlContentWrapper.BackColor = Color.White;
            pnlContentWrapper.Dock      = DockStyle.Fill;
            pnlContentWrapper.Controls.Add(pnlContentArea);
            pnlContentWrapper.Controls.Add(pnlContentHeader);

            // ── pnlContentHeader ────────────────────────────────────────────────
            pnlContentHeader.BackColor = Color.FromArgb(245, 248, 255);
            pnlContentHeader.Dock      = DockStyle.Top;
            pnlContentHeader.Height    = 54;
            pnlContentHeader.Controls.Add(lblContentTitle);
            pnlContentHeader.Controls.Add(lblContentIcon);

            lblContentIcon.AutoSize  = true;
            lblContentIcon.Font      = new Font("Segoe UI Emoji", 17F);
            lblContentIcon.ForeColor = Color.FromArgb(30, 58, 138);
            lblContentIcon.Location  = new Point(24, 9);
            lblContentIcon.Name      = "lblContentIcon";

            lblContentTitle.AutoSize  = true;
            lblContentTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblContentTitle.ForeColor = Color.FromArgb(23, 48, 107);
            lblContentTitle.Location  = new Point(62, 15);
            lblContentTitle.Name      = "lblContentTitle";

            // ── pnlContentArea ──────────────────────────────────────────────────
            pnlContentArea.AutoScroll = true;
            pnlContentArea.BackColor  = Color.White;
            pnlContentArea.Dock       = DockStyle.Fill;

            // ── pnlMain ─────────────────────────────────────────────────────────
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Controls.Add(pnlContentWrapper);
            pnlMain.Controls.Add(pnlSidebar);

            // ── pnlFooter ───────────────────────────────────────────────────────
            pnlFooter.BackColor = Color.FromArgb(240, 243, 250);
            pnlFooter.Dock      = DockStyle.Bottom;
            pnlFooter.Height    = 52;
            pnlFooter.Controls.Add(btnClose);

            // ── btnClose ────────────────────────────────────────────────────────
            btnClose.Anchor               = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.BackColor            = Color.FromArgb(30, 58, 138);
            btnClose.FlatStyle            = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize       = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(59, 90, 180);
            btnClose.Font                 = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnClose.ForeColor            = Color.White;
            btnClose.Location             = new Point(878, 10);
            btnClose.Size                 = new Size(110, 32);
            btnClose.Text                 = "✕   Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Cursor               = Cursors.Hand;
            btnClose.Click               += (_, _) => Close();

            // ── UserGuideForm ───────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(1000, 680);
            MinimumSize         = new Size(820, 540);
            Controls.Add(pnlMain);
            Controls.Add(pnlHeader);
            Controls.Add(pnlFooter);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox     = true;
            MinimizeBox     = false;
            Name            = "UserGuideForm";
            StartPosition   = FormStartPosition.CenterParent;
            Text            = "EOD Data Service — User Guide";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlMain.ResumeLayout(false);
            pnlContentWrapper.ResumeLayout(false);
            pnlContentHeader.ResumeLayout(false);
            pnlContentHeader.PerformLayout();
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
