using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EODService.DTOs.EOD;
using EODService.Persistance;
using EODSettingsApp.AppSettingsConfig;

namespace EODSettingsApp.Forms
{
    /// <summary>
    /// Modal dialog for querying, filtering, analyzing, and exporting historical market records from Oracle DB.
    /// </summary>
    public partial class HistoricalDataForm : Form
    {
        public HistoricalDataForm()
        {
            InitializeComponent();
            SetupGridColumns();
            InitializeFilters();
            LoadSymbols();
        }

        // ── Initialization ───────────────────────────────────────────────────────

        private void SetupGridColumns()
        {
            dgvHistory.Columns.Clear();
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "TickerID", HeaderText = "Ticker / Symbol", DataPropertyName = "TickerID" });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", DataPropertyName = "Date" });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "Open", HeaderText = "Open", DataPropertyName = "Open" });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "High", HeaderText = "High", DataPropertyName = "High" });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "Low", HeaderText = "Low", DataPropertyName = "Low" });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "Close", HeaderText = "Close", DataPropertyName = "Close" });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "AdjustedClose", HeaderText = "Adj. Close", DataPropertyName = "AdjustedClose" });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "Volume", HeaderText = "Volume", DataPropertyName = "Volume" });
        }

        private void InitializeFilters()
        {
            dtpFromDate.Value = DateTime.Today.AddDays(-30);
            dtpToDate.Value = DateTime.Today;
        }

        private void LoadSymbols()
        {
            try
            {
                cmbSymbol.Items.Clear();
                cmbSymbol.Items.Add("ALL");

                var model = AppSettingsService.Load();
                if (model.SymbolSettings?.Symbols != null)
                {
                    foreach (var symbol in model.SymbolSettings.Symbols)
                    {
                        if (!string.IsNullOrWhiteSpace(symbol) && !cmbSymbol.Items.Contains(symbol))
                        {
                            cmbSymbol.Items.Add(symbol.Trim().ToUpperInvariant());
                        }
                    }
                }

                cmbSymbol.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Warning loading symbols: {ex.Message}");
            }
        }

        // ── Search Action ────────────────────────────────────────────────────────

        private async void BtnSearchHistory_Click(object? sender, EventArgs e)
        {
            if (dtpFromDate.Value.Date > dtpToDate.Value.Date)
            {
                MessageBox.Show("From Date cannot be after To Date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSearchHistory.Enabled = false;
            btnSearchHistory.Text = "⏳ Searching...";
            SetStatus(success: true, "Querying Oracle database history...");
            dgvHistory.Rows.Clear();

            try
            {
                var connectionString = GetConnectionString();
                if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_DB_USER"))
                {
                    throw new Exception("Oracle database connection string is not configured properly.");
                }

                using var dbContext = AppDbContextFactory.Create(connectionString);

                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddTicks(-1);
                var selectedSymbol = cmbSymbol.SelectedItem?.ToString() ?? "ALL";

                // Query EodHistory table
                var historyQuery = dbContext.EodHistory.AsNoTracking().AsQueryable();

                if (!string.Equals(selectedSymbol, "ALL", StringComparison.OrdinalIgnoreCase))
                {
                    historyQuery = historyQuery.Where(h => h.TickerID.ToUpper() == selectedSymbol.ToUpper());
                }

                historyQuery = historyQuery.Where(h => h.Date >= fromDate && h.Date <= toDate);

                var historyList = await historyQuery.OrderByDescending(h => h.Date).ThenBy(h => h.TickerID).ToListAsync();
                var results = historyList.Cast<EodData>().ToList();

                // If EodHistory is empty, fallback check in EodDaily table
                if (!results.Any())
                {
                    var dailyQuery = dbContext.EodDaily.AsNoTracking().AsQueryable();

                    if (!string.Equals(selectedSymbol, "ALL", StringComparison.OrdinalIgnoreCase))
                    {
                        dailyQuery = dailyQuery.Where(d => d.TickerID.ToUpper() == selectedSymbol.ToUpper());
                    }

                    dailyQuery = dailyQuery.Where(d => d.Date >= fromDate && d.Date <= toDate);
                    var dailyList = await dailyQuery.OrderByDescending(d => d.Date).ThenBy(d => d.TickerID).ToListAsync();
                    results = dailyList.Cast<EodData>().ToList();
                }

                PopulateGridAndStats(results);
                SetStatus(success: true, $"✔ Found {results.Count} historical record(s).");
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Search failed: {ex.Message}");
                MessageBox.Show($"Database search error:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSearchHistory.Enabled = true;
                btnSearchHistory.Text = "🔍 Search History";
            }
        }

        private void PopulateGridAndStats(List<EodData> records)
        {
            dgvHistory.Rows.Clear();
            lblTotalRecords.Text = $"Records: {records.Count:N0}";

            if (!records.Any())
                return;

            foreach (var r in records)
            {
                dgvHistory.Rows.Add(
                    r.TickerID,
                    r.Date.ToString("yyyy-MM-dd"),
                    r.Open?.ToString("F4") ?? "-",
                    r.High?.ToString("F4") ?? "-",
                    r.Low?.ToString("F4") ?? "-",
                    r.Close?.ToString("F4") ?? "-",
                    r.AdjustedClose?.ToString("F4") ?? "-",
                    r.Volume?.ToString("N0") ?? "-"
                );
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private string GetConnectionString()
        {
            var model = AppSettingsService.Load();
            if (!string.IsNullOrWhiteSpace(model.ConnectionStrings?.DefaultConnection))
            {
                return model.ConnectionStrings.DefaultConnection;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(EODService.Config.PathesConfig.AppSettingsFileName, optional: true, reloadOnChange: false)
                .Build();

            return configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        private void SetStatus(bool success, string message)
        {
            lblHistoryStatus.ForeColor = success
                ? Color.FromArgb(22, 163, 74)   // Green
                : Color.FromArgb(185, 28, 28);  // Red
            lblHistoryStatus.Text = message;
        }
    }
}
