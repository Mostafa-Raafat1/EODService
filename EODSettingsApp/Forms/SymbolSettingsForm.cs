using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EODService.Persistance;
using EODSettingsApp.AppSettingsConfig;
using EODService.DTOs.Stock;

namespace EODSettingsApp.Forms
{
    /// <summary>
    /// Modal dialog for managing, updating, and deleting stock definitions stored in Oracle Database (EOD_STOCKS).
    /// </summary>
    public partial class SymbolSettingsForm : Form
    {
        private List<Stock> _stocks = new();
        private int _selectedStockId = 0;

        public SymbolSettingsForm()
        {
            InitializeComponent();
            AppIconHelper.ApplyAppIconAndTitle(this);
            SetupGridColumns();
            Load += async (_, _) => await LoadStockSettingsAsync();
        }

        private void SetupGridColumns()
        {
            dgvStocks.Columns.Clear();
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id", FillWeight = 25 });
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn { Name = "SC_Comp_Id", HeaderText = "Comp ID", DataPropertyName = "SC_Comp_Id", FillWeight = 35 });
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn { Name = "StockName", HeaderText = "Stock Name", DataPropertyName = "StockName", FillWeight = 135 });
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn { Name = "Isin", HeaderText = "ISIN", DataPropertyName = "Isin", FillWeight = 60 });
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn { Name = "YahooFinanceID", HeaderText = "Yahoo Ticker", DataPropertyName = "YahooFinanceID", FillWeight = 70 });
            dgvStocks.Columns.Add(new DataGridViewCheckBoxColumn { Name = "YahooFinanceExists", HeaderText = "YF Active", DataPropertyName = "YahooFinanceExists", FillWeight = 45 });
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn { Name = "TwelveDataID", HeaderText = "Twelve Data Ticker", DataPropertyName = "TwelveDataID", FillWeight = 70 });
            dgvStocks.Columns.Add(new DataGridViewCheckBoxColumn { Name = "TwelveDataExists", HeaderText = "TD Active", DataPropertyName = "TwelveDataExists", FillWeight = 45 });
            dgvStocks.Columns.Add(new DataGridViewTextBoxColumn { Name = "StockExchange", HeaderText = "Exchange", DataPropertyName = "StockExchange", FillWeight = 50 });
        }

        private async Task LoadStockSettingsAsync()
        {
            try
            {
                SetStatus(success: true, "Loading stock configurations from Oracle DB...");
                var connectionString = ConnectionStringResolver.Get();
                if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_DB_USER"))
                {
                    throw new Exception("Oracle database connection string is missing or invalid.");
                }

                using var dbContext = AppDbContextFactory.Create(connectionString);
                _stocks = await dbContext.Stock.AsNoTracking().OrderBy(s => s.Id).ToListAsync();

                PopulateGrid();

                if (_stocks.Any())
                {
                    dgvStocks.Rows[0].Selected = true;
                    SetStatus(success: true, $"✔ Loaded {_stocks.Count} stock record(s) from Oracle DB (EOD_STOCKS).");
                }
                else
                {
                    ClearEditForm();
                    SetStatus(success: false, "No stocks found in database.");
                }
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Failed loading database stocks: {ex.Message}");
            }
        }

        private void PopulateGrid()
        {
            dgvStocks.Rows.Clear();
            foreach (var s in _stocks)
            {
                dgvStocks.Rows.Add(
                    s.Id,
                    s.SC_Comp_Id,
                    s.StockName,
                    s.ISIN ?? "-",
                    s.YahooFinanceID ?? "-",
                    s.YahooFinanceExists,
                    s.TwelveDataID ?? "-",
                    s.TwelveDataExists,
                    s.StockExchange ?? "-"
                );
            }
        }

        private void DgvStocks_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvStocks.SelectedRows.Count == 0)
            {
                ClearEditForm();
                return;
            }

            var row = dgvStocks.SelectedRows[0];
            if (row.Cells[0].Value == null) return;

            int id = Convert.ToInt32(row.Cells[0].Value);
            var stock = _stocks.FirstOrDefault(s => s.Id == id);
            if (stock != null)
            {
                _selectedStockId = stock.Id;
                txtStockId.Text = stock.Id.ToString();
                txtCompId.Text = stock.SC_Comp_Id.ToString();
                txtStockName.Text = stock.StockName;
                txtIsin.Text = stock.ISIN ?? "";
                txtYahooId.Text = stock.YahooFinanceID ?? "";
                chkYahooActive.Checked = stock.YahooFinanceExists;
                txtTwelveDataId.Text = stock.TwelveDataID ?? "";
                chkTwelveDataActive.Checked = stock.TwelveDataExists;
                txtExchange.Text = stock.StockExchange ?? "";

                UpdateProviderControlsState();

                grpEditStock.Enabled = true;
                btnUpdateStock.Enabled = true;
                btnRemoveSymbol.Enabled = true;
            }
        }

        private void ChkYahooActive_CheckedChanged(object? sender, EventArgs e)
        {
            txtYahooId.Enabled = chkYahooActive.Checked;
            txtYahooId.BackColor = chkYahooActive.Checked ? Color.White : Color.FromArgb(241, 245, 249);
        }

        private void ChkTwelveDataActive_CheckedChanged(object? sender, EventArgs e)
        {
            txtTwelveDataId.Enabled = chkTwelveDataActive.Checked;
            txtTwelveDataId.BackColor = chkTwelveDataActive.Checked ? Color.White : Color.FromArgb(241, 245, 249);
        }

        private void UpdateProviderControlsState()
        {
            txtYahooId.Enabled = chkYahooActive.Checked;
            txtYahooId.BackColor = chkYahooActive.Checked ? Color.White : Color.FromArgb(241, 245, 249);
            txtTwelveDataId.Enabled = chkTwelveDataActive.Checked;
            txtTwelveDataId.BackColor = chkTwelveDataActive.Checked ? Color.White : Color.FromArgb(241, 245, 249);
        }

        private void BtnUpdateStock_Click(object? sender, EventArgs e)
        {
            if (_selectedStockId == 0)
            {
                SetStatus(success: false, "✘ Please select a stock from the grid to update.");
                return;
            }

            var stock = _stocks.FirstOrDefault(s => s.Id == _selectedStockId);
            if (stock == null) return;

            var name = txtStockName.Text.Trim();
            var compIdText = txtCompId.Text.Trim();
            var isin = txtIsin.Text.Trim().ToUpperInvariant();
            var yahooId = txtYahooId.Text.Trim().ToUpperInvariant();
            var twelveDataId = txtTwelveDataId.Text.Trim().ToUpperInvariant();
            var exchange = txtExchange.Text.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(name))
            {
                SetStatus(success: false, "✘ Stock Name cannot be empty.");
                return;
            }

            if (!int.TryParse(compIdText, out var compId))
            {
                SetStatus(success: false, "✘ Company ID must be a valid integer number (e.g. 687, 10).");
                return;
            }

            if (!chkYahooActive.Checked && !chkTwelveDataActive.Checked)
            {
                SetStatus(success: false, "✘ At least one provider (Yahoo Finance or Twelve Data) must be enabled.");
                return;
            }

            if (chkYahooActive.Checked && string.IsNullOrWhiteSpace(yahooId))
            {
                SetStatus(success: false, "✘ Yahoo Finance Ticker is required when Yahoo Active is enabled.");
                return;
            }

            if (chkTwelveDataActive.Checked && string.IsNullOrWhiteSpace(twelveDataId))
            {
                SetStatus(success: false, "✘ Twelve Data Ticker is required when TD Active is enabled.");
                return;
            }

            stock.StockName = name;
            stock.SC_Comp_Id = compId;
            stock.ISIN = string.IsNullOrWhiteSpace(isin) ? string.Empty : isin;
            stock.YahooFinanceID = string.IsNullOrWhiteSpace(yahooId) ? null : yahooId;
            stock.YahooFinanceExists = chkYahooActive.Checked;
            stock.TwelveDataID = string.IsNullOrWhiteSpace(twelveDataId) ? null : twelveDataId;
            stock.TwelveDataExists = chkTwelveDataActive.Checked;
            stock.StockExchange = string.IsNullOrWhiteSpace(exchange) ? null : exchange;

            PopulateGrid();
            SelectGridRowById(_selectedStockId);

            SetStatus(success: true, $"✔ Updated '{name}' (Comp ID: {compId}). Click 'Save Database Changes' to commit to Oracle DB.");
        }

        private void BtnRemoveSymbol_Click(object? sender, EventArgs e)
        {
            if (_selectedStockId == 0)
            {
                SetStatus(success: false, "✘ Please select a stock from the grid to delete.");
                return;
            }

            var stock = _stocks.FirstOrDefault(s => s.Id == _selectedStockId);
            if (stock == null) return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete stock '{stock.StockName}' (ID: {stock.Id}) from the database?",
                "Confirm Delete Stock",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                _stocks.Remove(stock);
                PopulateGrid();

                if (_stocks.Any())
                {
                    dgvStocks.Rows[0].Selected = true;
                }
                else
                {
                    ClearEditForm();
                }

                SetStatus(success: true, $"✔ Removed '{stock.StockName}'. Click 'Save Database Changes' to commit deletion.");
            }
        }

        private async void BtnSaveSymbolSettings_Click(object? sender, EventArgs e)
        {
            try
            {
                SetStatus(success: true, "Saving stock changes to Oracle Database...");
                var connectionString = ConnectionStringResolver.Get();   
                using var dbContext = AppDbContextFactory.Create(connectionString);

                // Reconcile EOD_STOCKS table
                var dbStocks = await dbContext.Stock.ToListAsync();

                // Remove deleted stocks
                var currentIds = _stocks.Select(s => s.Id).ToHashSet();
                var toRemove = dbStocks.Where(s => !currentIds.Contains(s.Id)).ToList();
                if (toRemove.Any())
                {
                    dbContext.Stock.RemoveRange(toRemove);
                }

                // Update existing stocks
                foreach (var s in _stocks)
                {
                    var existing = dbStocks.FirstOrDefault(x => x.Id == s.Id);
                    if (existing != null)
                    {
                        existing.StockName = s.StockName;
                        existing.SC_Comp_Id = s.SC_Comp_Id;
                        existing.ISIN = s.ISIN;
                        existing.YahooFinanceID = s.YahooFinanceID;
                        existing.YahooFinanceExists = s.YahooFinanceExists;
                        existing.TwelveDataID = s.TwelveDataID;
                        existing.TwelveDataExists = s.TwelveDataExists;
                        existing.StockExchange = s.StockExchange;
                    }
                }

                await dbContext.SaveChangesAsync();

                // Keep AppSettings.json in sync for legacy compatibility
                var model = AppSettingsService.Load();
                model.SymbolSettings = new SymbolSettingsSection
                {
                    Symbols = _stocks.Where(s => !string.IsNullOrWhiteSpace(s.TwelveDataID)).Select(s => s.TwelveDataID!).ToList()
                };
                AppSettingsService.Save(model);

                SetStatus(success: true, "✔ All stock changes saved successfully to Oracle DB (EOD_STOCKS).");
                MessageBox.Show("Stock configurations saved successfully to Oracle Database!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Database save failed: {ex.Message}");
                MessageBox.Show($"Failed saving stocks to Oracle DB:\n\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectGridRowById(int stockId)
        {
            foreach (DataGridViewRow row in dgvStocks.Rows)
            {
                if (row.Cells[0].Value != null && Convert.ToInt32(row.Cells[0].Value) == stockId)
                {
                    row.Selected = true;
                    dgvStocks.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private void ClearEditForm()
        {
            _selectedStockId = 0;
            txtStockId.Clear();
            txtCompId.Clear();
            txtStockName.Clear();
            txtIsin.Clear();
            txtYahooId.Clear();
            chkYahooActive.Checked = false;
            txtTwelveDataId.Clear();
            chkTwelveDataActive.Checked = false;
            txtExchange.Clear();

            grpEditStock.Enabled = false;
            btnUpdateStock.Enabled = false;
            btnRemoveSymbol.Enabled = false;
        }

        private void SetStatus(bool success, string message)
        {
            lblSymbolSettingsStatus.ForeColor = success
                ? Color.FromArgb(22, 163, 74)   // Green
                : Color.FromArgb(185, 28, 28);  // Red
            lblSymbolSettingsStatus.Text = message;
        }
    }
}
