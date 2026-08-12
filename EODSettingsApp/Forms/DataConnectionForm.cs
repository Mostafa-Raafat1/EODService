using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EODService.Persistance;

namespace EODSettingsApp.Forms
{
    /// <summary>
    /// Form for configuring and testing Oracle Database connection settings.
    /// </summary>
    public partial class DataConnectionForm : Form
    {
        public DataConnectionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the constructed Oracle Connection String based on the input fields.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                var host = string.IsNullOrWhiteSpace(txtHost.Text) ? "10.120.143.51" : txtHost.Text.Trim();
                var port = string.IsNullOrWhiteSpace(txtPort.Text) ? "1521" : txtPort.Text.Trim();
                var serviceName = string.IsNullOrWhiteSpace(txtServiceName.Text) ? "cibcorclhq" : txtServiceName.Text.Trim();
                var username = string.IsNullOrWhiteSpace(txtUsername.Text) ? "intern" : txtUsername.Text.Trim();
                var password = string.IsNullOrWhiteSpace(txtPassword.Text) ? "intern" : txtPassword.Text.Trim();

                return $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={serviceName})));User Id={username};Password={password};";
            }
        }

        // ── Test Connection Event ────────────────────────────────────────────────
        private async void BtnTestConnection_Click(object? sender, EventArgs e)
        {
            var connStr = ConnectionString;
            SetStatus(Color.FromArgb(30, 58, 138), "● Connection Status: Testing connection...");
            SetUiBusy(true);

            try
            {
                bool canConnect = await Task.Run(async () =>
                {
                    using var dbContext = AppDbContextFactory.Create(connStr);
                    return await dbContext.Database.CanConnectAsync();
                });

                if (canConnect)
                {
                    SetStatus(Color.FromArgb(22, 163, 74), "● Connection Status: Connected successfully");
                }
                else
                {
                    SetStatus(Color.FromArgb(185, 28, 28), "● Connection Status: Unable to reach database server");
                }
            }
            catch (Exception ex)
            {
                SetStatus(Color.FromArgb(185, 28, 28), $"● Connection Status: Error ({ex.Message})");
            }
            finally
            {
                SetUiBusy(false);
            }
        }

        // ── Save Event ───────────────────────────────────────────────────────────
        private void BtnSave_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        // ── Cancel Event ─────────────────────────────────────────────────────────
        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────
        private void SetStatus(Color color, string statusText)
        {
            lblStatus.ForeColor = color;
            lblStatus.Text = statusText;
        }

        private void SetUiBusy(bool busy)
        {
            btnTestConnection.Enabled = !busy;
            btnSave.Enabled = !busy;
            btnCancel.Enabled = !busy;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }
    }
}
