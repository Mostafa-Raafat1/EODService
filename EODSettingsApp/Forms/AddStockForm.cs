using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EODService.DTOs.Stock;
using EODService.Persistance;
using EODSettingsApp.AppSettingsConfig;

namespace EODSettingsApp.Forms
{
    public partial class AddStockForm : Form
    {
        public AddStockForm()
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

                int.TryParse(txtInitialId.Text.Trim(), out int initialId);

                var newStock = new Stock
                {
                    StockName = stockName,
                    SC_Comp_Id = initialId,
                    StockExchange = string.IsNullOrWhiteSpace(txtExchange.Text) ? null : txtExchange.Text.Trim(),
                    TwelveDataExists = rdoTdYes.Checked,
                    YahooFinanceExists = rdoYfYes.Checked,
                    ReuterExists = rdoLsegYes.Checked,
                    TwelveDataID = string.IsNullOrWhiteSpace(txtTdSymbol.Text) ? null : txtTdSymbol.Text.Trim(),
                    YahooFinanceID = string.IsNullOrWhiteSpace(txtYfSymbol.Text) ? null : txtYfSymbol.Text.Trim(),
                    ReuterID = string.IsNullOrWhiteSpace(txtLsegSymbol.Text) ? null : txtLsegSymbol.Text.Trim(),
                    ISIN = string.IsNullOrWhiteSpace(txtIsin.Text) ? string.Empty : txtIsin.Text.Trim()
                };

                dbContext.Stock.Add(newStock);
                await dbContext.SaveChangesAsync();

                SetStatus(success: true, $"✔ Stock '{stockName}' added successfully!");
                ClearForm();
            }
            catch (Exception ex)
            {
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
            rdoLsegYes.Checked = true;
            txtTdSymbol.Clear();
            txtYfSymbol.Clear();
            txtLsegSymbol.Clear();
            txtIsin.Clear();
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

