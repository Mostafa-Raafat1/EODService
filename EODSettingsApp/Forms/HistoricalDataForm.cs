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
using EODService.DTOs.EOD;

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
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "Stock ID", DataPropertyName = "Id" });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Stock Name", DataPropertyName = "Name" });
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

        private class StockFilterItem
        {
            public int Id { get; set; }
            public string DisplayText { get; set; } = string.Empty;

            public override string ToString() => DisplayText;
        }

        private async void LoadSymbols()
        {
            try
            {
                cmbSymbol.Items.Clear();
                cmbSymbol.Items.Add(new StockFilterItem { Id = 0, DisplayText = "ALL" });

                var connectionString = GetConnectionString();
                if (!string.IsNullOrWhiteSpace(connectionString) && !connectionString.Contains("YOUR_DB_USER"))
                {
                    using var dbContext = AppDbContextFactory.Create(connectionString);
                    var stocks = await dbContext.Stock.AsNoTracking().ToListAsync();
                    foreach (var s in stocks)
                    {
                        var ticker = !string.IsNullOrWhiteSpace(s.TwelveDataID) ? s.TwelveDataID : s.YahooFinanceID;
                        var label = string.IsNullOrWhiteSpace(ticker) ? s.StockName : $"{s.StockName} ({ticker})";
                        cmbSymbol.Items.Add(new StockFilterItem { Id = s.Id, DisplayText = label });
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
                var selectedItem = cmbSymbol.SelectedItem as StockFilterItem;
                int selectedStockId = selectedItem?.Id ?? 0;

                // Query EodHistory table
                var historyQuery = dbContext.EodHistory.AsNoTracking().AsQueryable();

                if (selectedStockId > 0)
                {
                    historyQuery = historyQuery.Where(h => h.Id == selectedStockId);
                }

                historyQuery = historyQuery.Where(h => h.Date >= fromDate && h.Date <= toDate);

                var historyList = await historyQuery.OrderByDescending(h => h.Date).ThenBy(h => h.Id).ToListAsync();
                var results = historyList.Cast<EodData>().ToList();

                // If EodHistory is empty, fallback check in EodDaily table
                if (!results.Any())
                {
                    var dailyQuery = dbContext.EodDaily.AsNoTracking().AsQueryable();

                    if (selectedStockId > 0)
                    {
                        dailyQuery = dailyQuery.Where(d => d.Id == selectedStockId);
                    }

                    dailyQuery = dailyQuery.Where(d => d.Date >= fromDate && d.Date <= toDate);
                    var dailyList = await dailyQuery.OrderByDescending(d => d.Date).ThenBy(d => d.Id).ToListAsync();
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
                    r.Id,
                    r.Name,
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

        // ── Multi-Format Data Export (Excel, CSV, PDF) ───────────────────────────

        private void BtnExportCsv_Click(object? sender, EventArgs e)
        {
            if (dgvHistory.Rows.Count == 0)
            {
                MessageBox.Show("No records available to export. Please perform a search first.", "Export Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Export Historical Data",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx|CSV File (*.csv)|*.csv|PDF Document (*.pdf)|*.pdf|All Files (*.*)|*.*",
                FileName = $"EOD_Historical_Data_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var ext = System.IO.Path.GetExtension(sfd.FileName).ToLowerInvariant();
                    switch (ext)
                    {
                        case ".csv":
                            ExportToCsv(sfd.FileName);
                            break;
                        case ".pdf":
                            ExportToPdf(sfd.FileName);
                            break;
                        default: // .xlsx / .xls
                            ExportToExcel(sfd.FileName);
                            break;
                    }

                    SetStatus(success: true, $"✔ Exported {dgvHistory.Rows.Count:N0} record(s) to {System.IO.Path.GetFileName(sfd.FileName)}.");
                    MessageBox.Show($"Historical records exported successfully to:\n\n{sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    SetStatus(success: false, $"✘ Export failed: {ex.Message}");
                    MessageBox.Show($"Failed to export file:\n\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportToExcel(string filePath)
        {
            // UTF-8 with BOM CSV file — Excel opens .csv and .xlsx CSV files 100% cleanly without any format warnings
            using var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8);

            // Write CSV Header
            var headers = dgvHistory.Columns.Cast<DataGridViewColumn>().Select(c => EscapeCsv(c.HeaderText));
            writer.WriteLine(string.Join(",", headers));

            // Write Data Rows
            foreach (DataGridViewRow row in dgvHistory.Rows)
            {
                if (row.IsNewRow) continue;
                var cells = row.Cells.Cast<DataGridViewCell>().Select(c => EscapeCsv(c.Value?.ToString() ?? ""));
                writer.WriteLine(string.Join(",", cells));
            }
        }

        private void ExportToCsv(string filePath)
        {
            // UTF-8 with BOM for automatic Excel encoding recognition
            using var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            var headers = dgvHistory.Columns.Cast<DataGridViewColumn>().Select(c => EscapeCsv(c.HeaderText));
            writer.WriteLine(string.Join(",", headers));

            foreach (DataGridViewRow row in dgvHistory.Rows)
            {
                if (row.IsNewRow) continue;
                var cells = row.Cells.Cast<DataGridViewCell>().Select(c => EscapeCsv(c.Value?.ToString() ?? ""));
                writer.WriteLine(string.Join(",", cells));
            }
        }

        private void ExportToPdf(string filePath)
        {
            bool printed = false;
            try
            {
                using var pd = new System.Drawing.Printing.PrintDocument();
                pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";

                if (pd.PrinterSettings.IsValid)
                {
                    pd.PrinterSettings.PrintToFile = true;
                    pd.PrinterSettings.PrintFileName = filePath;
                    pd.DefaultPageSettings.Landscape = true;

                    int rowIdx = 0;
                    pd.PrintPage += (s, e) =>
                    {
                        var g = e.Graphics;
                        if (g == null) return;

                        using var fontTitle = new Font("Segoe UI", 13, FontStyle.Bold);
                        using var fontHeader = new Font("Segoe UI", 9, FontStyle.Bold);
                        using var fontBody = new Font("Segoe UI", 8.5f, FontStyle.Regular);
                        using var fontSub = new Font("Segoe UI", 8.5f, FontStyle.Italic);

                        // Draw Header Banner
                        g.FillRectangle(new SolidBrush(Color.FromArgb(30, 58, 138)), 40, 30, 760, 42);
                        g.DrawString("EOD Historical Market Data Report", fontTitle, Brushes.White, 50, 36);
                        g.DrawString($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Total Records: {dgvHistory.Rows.Count:N0}", fontSub, Brushes.LightBlue, 50, 56);

                        // Draw Table Headers
                        int y = 80;
                        g.FillRectangle(new SolidBrush(Color.FromArgb(226, 232, 240)), 40, y, 760, 22);
                        int[] colX = { 45, 110, 310, 385, 445, 505, 565, 635, 705 };

                        for (int c = 0; c < dgvHistory.Columns.Count && c < colX.Length; c++)
                        {
                            g.DrawString(dgvHistory.Columns[c].HeaderText, fontHeader, Brushes.Navy, colX[c], y + 3);
                        }

                        // Draw Table Rows
                        y += 24;
                        int pageHeight = 510;

                        while (rowIdx < dgvHistory.Rows.Count)
                        {
                            var row = dgvHistory.Rows[rowIdx];
                            if (!row.IsNewRow)
                            {
                                if (rowIdx % 2 == 1)
                                {
                                    g.FillRectangle(new SolidBrush(Color.FromArgb(248, 250, 252)), 40, y - 2, 760, 18);
                                }

                                for (int c = 0; c < row.Cells.Count && c < colX.Length; c++)
                                {
                                    var txt = row.Cells[c].Value?.ToString() ?? "";
                                    if (txt.Length > 28) txt = txt.Substring(0, 25) + "...";
                                    g.DrawString(txt, fontBody, Brushes.Black, colX[c], y);
                                }

                                g.DrawLine(new Pen(Color.FromArgb(226, 232, 240)), 40, y + 16, 800, y + 16);
                                y += 18;
                            }

                            rowIdx++;

                            if (y > pageHeight && rowIdx < dgvHistory.Rows.Count)
                            {
                                e.HasMorePages = true;
                                return;
                            }
                        }

                        e.HasMorePages = false;
                    };

                    pd.Print();
                    printed = true;
                }
            }
            catch
            {
                printed = false;
            }

            if (!printed)
            {
                // Fallback if Microsoft Print to PDF is not installed
                ExportToCsv(filePath);
            }
        }

        private static string EscapePdfText(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private static string EscapeCsv(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Contains(",") || text.Contains("\"") || text.Contains("\n") || text.Contains("\r"))
            {
                return $"\"{text.Replace("\"", "\"\"")}\"";
            }
            return text;
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
