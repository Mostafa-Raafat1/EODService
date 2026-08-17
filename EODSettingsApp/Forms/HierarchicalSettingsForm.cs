using System;
using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    public partial class HierarchicalSettingsForm : Form
    {
        private Form? _activeForm = null;

        public HierarchicalSettingsForm()
        {
            InitializeComponent();
            InitializeCategories();
        }

        private void InitializeCategories()
        {
            // Clear existing nodes
            tvCategories.Nodes.Clear();

            // Create root node
            var rootNode = new TreeNode("Settings")
            {
                NodeFont = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Tag = "ROOT"
            };

            // Create provider parent node with sub-nodes
            var providerNode = new TreeNode("Provider Settings") { Tag = "PROVIDER" };
            providerNode.Nodes.Add(new TreeNode("Yahoo Finance") { Tag = "PROVIDER_YAHOO" });
            providerNode.Nodes.Add(new TreeNode("TwelveData") { Tag = "PROVIDER_TWELVE" });

            // Create child nodes
            rootNode.Nodes.Add(providerNode);
            rootNode.Nodes.Add(new TreeNode("Symbol Settings") { Tag = "SYMBOL" });
            rootNode.Nodes.Add(new TreeNode("Database Connection") { Tag = "DATABASE" });
            rootNode.Nodes.Add(new TreeNode("Add Stock") { Tag = "ADD_STOCK" });

            tvCategories.Nodes.Add(rootNode);
        }

        private void HierarchicalSettingsForm_Load(object sender, EventArgs e)
        {
            // Expand all categories on load
            tvCategories.ExpandAll();

            // Automatically select Yahoo Finance node (the first leaf node under Provider Settings) on load
            if (tvCategories.Nodes.Count > 0 && 
                tvCategories.Nodes[0].Nodes.Count > 0 && 
                tvCategories.Nodes[0].Nodes[0].Nodes.Count > 0)
            {
                tvCategories.SelectedNode = tvCategories.Nodes[0].Nodes[0].Nodes[0];
            }
        }

        private void TvCategories_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null) return;

            string? tag = e.Node.Tag as string;
            // Toggle expansion state for parent nodes on click
            if (tag == "ROOT" || tag == "PROVIDER")
            {
                if (e.Node.IsExpanded)
                {
                    e.Node.Collapse();
                }
                else
                {
                    e.Node.Expand();
                }
            }
        }

        private void TvCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;

            string? tag = e.Node.Tag as string;

            switch (tag)
            {
                case "PROVIDER_YAHOO":
                    ShowChildForm(new ProviderSettingsForm(0));
                    break;
                case "PROVIDER_TWELVE":
                    ShowChildForm(new ProviderSettingsForm(1));
                    break;
                case "SYMBOL":
                    ShowChildForm(new SymbolSettingsForm());
                    break;
                case "DATABASE":
                    ShowChildForm(new DatabaseSettingsForm());
                    break;
                case "ADD_STOCK":
                    ShowChildForm(new AddStockForm());
                    break;
                default:
                    // ROOT and PROVIDER nodes are expansion toggle items and do not change form views
                    break;
            }
        }

        private void ShowChildForm(Form childForm)
        {
            // Close and clean up previous active form
            if (_activeForm != null)
            {
                _activeForm.Close();
                _activeForm.Dispose();
            }

            _activeForm = childForm;

            // Configure child form for embedding
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            // Add margin padding to create breathing room between the tree and the form content
            pnlContent.Padding = new Padding(12);
            pnlContent.BackColor = Color.FromArgb(241, 245, 249); // Matches sidebar for a clean, unified outer border

            // Calculate parent size to fit child form exactly + the content padding
            int targetWidth = pnlSidebar.Width + childForm.Width + pnlContent.Padding.Horizontal;
            int targetHeight = Math.Max(520, childForm.Height + pnlContent.Padding.Vertical);

            // Dynamically resize parent window
            this.ClientSize = new Size(targetWidth, targetHeight);

            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(childForm);
            
            childForm.Show();
        }

        private void TvCategories_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            bool isSelected = (e.State & TreeNodeStates.Selected) != 0;
            bool isHovered = (e.State & TreeNodeStates.Hot) != 0;

            Color backColor;
            Color foreColor;

            if (isSelected)
            {
                backColor = Color.FromArgb(30, 58, 138); // Theme dark blue for selection
                foreColor = Color.White;
            }
            else if (isHovered)
            {
                backColor = Color.FromArgb(226, 232, 240); // Hover light gray
                foreColor = Color.FromArgb(15, 23, 42);
            }
            else
            {
                backColor = tvCategories.BackColor;
                foreColor = tvCategories.ForeColor;
            }

            // Draw background for full row width
            using (var brush = new SolidBrush(backColor))
            {
                var rect = new Rectangle(0, e.Bounds.Top, tvCategories.Width, e.Bounds.Height);
                e.Graphics.FillRectangle(brush, rect);
            }

            // Calculate indentation based on node level (Level 0: 30, Level 1: 55, Level 2: 80)
            int indent = e.Node.Level * 25 + 30;
            var textRect = new Rectangle(indent, e.Bounds.Top, tvCategories.Width - indent, e.Bounds.Height);

            // Draw expand/collapse arrow if the node has children
            if (e.Node.Nodes.Count > 0)
            {
                int arrowSize = 6;
                int arrowX = indent - 18;
                int arrowY = e.Bounds.Top + (e.Bounds.Height / 2) - (arrowSize / 2);

                using (var brush = new SolidBrush(foreColor))
                {
                    if (e.Node.IsExpanded)
                    {
                        // Downward filled triangle
                        Point[] points = {
                            new Point(arrowX, arrowY + 1),
                            new Point(arrowX + arrowSize, arrowY + 1),
                            new Point(arrowX + (arrowSize / 2), arrowY + 1 + (arrowSize / 2))
                        };
                        e.Graphics.FillPolygon(brush, points);
                    }
                    else
                    {
                        // Rightward filled triangle
                        Point[] points = {
                            new Point(arrowX + 1, arrowY),
                            new Point(arrowX + 1 + (arrowSize / 2), arrowY + (arrowSize / 2)),
                            new Point(arrowX + 1, arrowY + arrowSize)
                        };
                        e.Graphics.FillPolygon(brush, points);
                    }
                }
            }

            // Draw text
            TextRenderer.DrawText(
                e.Graphics,
                e.Node.Text,
                e.Node.NodeFont ?? tvCategories.Font,
                textRect,
                foreColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left
            );
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // Properly dispose the embedded active form
            if (_activeForm != null)
            {
                _activeForm.Close();
                _activeForm.Dispose();
                _activeForm = null;
            }
        }
    }
}
