using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    partial class HierarchicalSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlSidebar;
        private Label lblSidebarHeader;
        private TreeView tvCategories;
        private Panel pnlContent;

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
            pnlSidebar = new Panel();
            lblSidebarHeader = new Label();
            tvCategories = new TreeView();
            pnlContent = new Panel();
            
            pnlSidebar.SuspendLayout();
            SuspendLayout();

            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(241, 245, 249);
            pnlSidebar.Controls.Add(tvCategories);
            pnlSidebar.Controls.Add(lblSidebarHeader);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(220, 600);
            pnlSidebar.TabIndex = 0;
            // 
            // lblSidebarHeader
            // 
            lblSidebarHeader.BackColor = Color.FromArgb(30, 58, 138);
            lblSidebarHeader.Dock = DockStyle.Top;
            lblSidebarHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblSidebarHeader.ForeColor = Color.White;
            lblSidebarHeader.Location = new Point(0, 0);
            lblSidebarHeader.Name = "lblSidebarHeader";
            lblSidebarHeader.Size = new Size(220, 50);
            lblSidebarHeader.TabIndex = 0;
            lblSidebarHeader.Text = "Settings Categories";
            lblSidebarHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tvCategories
            // 
            tvCategories.BackColor = Color.FromArgb(241, 245, 249);
            tvCategories.BorderStyle = BorderStyle.None;
            tvCategories.Dock = DockStyle.Fill;
            tvCategories.DrawMode = TreeViewDrawMode.OwnerDrawText;
            tvCategories.Font = new Font("Segoe UI", 10F);
            tvCategories.ForeColor = Color.FromArgb(51, 65, 85);
            tvCategories.FullRowSelect = true;
            tvCategories.HotTracking = true;
            tvCategories.Indent = 15;
            tvCategories.ItemHeight = 40;
            tvCategories.Location = new Point(0, 50);
            tvCategories.Name = "tvCategories";
            tvCategories.ShowLines = false;
            tvCategories.ShowPlusMinus = false;
            tvCategories.ShowRootLines = false;
            tvCategories.Size = new Size(220, 550);
            tvCategories.TabIndex = 1;
            tvCategories.DrawNode += TvCategories_DrawNode;
            tvCategories.AfterSelect += TvCategories_AfterSelect;
            tvCategories.NodeMouseClick += TvCategories_NodeMouseClick;
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.FromArgb(248, 250, 252);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(220, 0);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(600, 600);
            pnlContent.TabIndex = 1;
            // 
            // HierarchicalSettingsForm
            // 
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(820, 600);
            Controls.Add(pnlContent);
            Controls.Add(pnlSidebar);
            Font = new Font("Segoe UI", 9.5F);
            MinimumSize = new Size(400, 300);
            Name = "HierarchicalSettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "TICKR";
            Load += HierarchicalSettingsForm_Load;
            pnlSidebar.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
