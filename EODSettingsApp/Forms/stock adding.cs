using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EODService.DTOs.Stock;
using EODService.Persistance;
using EODSettingsApp.AppSettingsConfig;

namespace EODSettingsApp.Forms
{
    public partial class stockadd : Form
    {
        public stockadd()
        {
            InitializeComponent();
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var stockName = txtStockName.Text.Trim();
            if (string.IsNullOrWhiteSpace(stockName))
            {
                SetStatus(success: false, "✘ Please enter a Stock Name.");
                txtStockName.Focus();
                return;
            }

            btnAdd.Enabled = false;
            btnAdd.Text = "Saving...";
            SetStatus(success: true, "Saving stock to database...");

            try
            {
                var appSettings = AppSettingsService.Load();
                var connectionString = appSettings.ConnectionStrings?.DefaultConnection;

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    SetStatus(success: false, "✘ DB connection not configured. Go to Settings → Database.");
                    return;
                }

                using var dbContext = AppDbContextFactory.Create(connectionString);

                var newStock = new EodStock
                {
                    StockName = stockName,
                    InitialId = string.IsNullOrWhiteSpace(txtInitialId.Text) ? null : txtInitialId.Text.Trim(),
                    Exchange  = string.IsNullOrWhiteSpace(txtExchange.Text)  ? null : txtExchange.Text.Trim(),
                    TdTradable = rdoTdYes.Checked,
                    YfTradable = rdoYfYes.Checked,
                    TdSymbol  = string.IsNullOrWhiteSpace(txtTdSymbol.Text)  ? null : txtTdSymbol.Text.Trim(),
                    YfSymbol  = string.IsNullOrWhiteSpace(txtYfSymbol.Text)  ? null : txtYfSymbol.Text.Trim()
                };

                dbContext.EodStocks.Add(newStock);
                await dbContext.SaveChangesAsync();

                SetStatus(success: true, $"✔ Stock '{stockName}' added successfully!");
                ClearForm();
            }
            catch (Exception ex)
            {
                // Show the deepest inner exception for better Oracle diagnostics
                var inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;

                SetStatus(success: false, $"✘ {inner.Message}");
            }
            finally
            {
                btnAdd.Enabled = true;
                btnAdd.Text = "Add Stock";
            }
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            ClearForm();
            lblStatus.Text = "";
        }

        private void ClearForm()
        {
            txtStockName.Clear();
            txtInitialId.Clear();
            txtExchange.Clear();
            rdoTdYes.Checked = true;
            rdoYfYes.Checked = true;
            txtTdSymbol.Clear();
            txtYfSymbol.Clear();
            txtStockName.Focus();
        }

        private void SetStatus(bool success, string message)
        {
            lblStatus.ForeColor = success
                ? Color.FromArgb(22, 163, 74)   // Green
                : Color.FromArgb(185, 28, 28);  // Red
            lblStatus.Text = message;
        }
    }
}
