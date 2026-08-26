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
        private const int MaxDisplayRecords = 2000;

        public HistoricalDataForm()
        {
            InitializeComponent();
            AppIconHelper.ApplyAppIconAndTitle(this);
            SetupGridColumns();
            InitializeFilters();
            Load += async (_, _) => await LoadSymbolsAsync();
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
            public string StockName { get; set; } = string.Empty;

            public override string ToString()
            {
                if (Id == 0) return "ALL";
                return StockName;
            }
        }

        private async Task LoadSymbolsAsync()
        {
            try
            {
                cmbSymbol.Items.Clear();
                cmbSymbol.Items.Add(new StockFilterItem { Id = 0, StockName = "ALL" });

                var connectionString = ConnectionStringResolver.Get();
                if (!string.IsNullOrWhiteSpace(connectionString) && !connectionString.Contains("YOUR_DB_USER"))
                {
                    using var dbContext = AppDbContextFactory.Create(connectionString);
                    var stocks = await dbContext.Stock
                        .AsNoTracking()
                        .OrderBy(s => s.StockName)
                        .ToListAsync();

                    var addedIds = new HashSet<int>();

                    foreach (var stock in stocks)
                    {
                        if (addedIds.Contains(stock.Id))
                            continue;

                        var name = stock.StockName?.Trim();
                        if (string.IsNullOrWhiteSpace(name))
                            continue;

                        cmbSymbol.Items.Add(new StockFilterItem
                        {
                            Id = stock.Id,
                            StockName = name
                        });

                        addedIds.Add(stock.Id);
                    }
                }

                cmbSymbol.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Warning loading symbols from DB: {ex.Message}");
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
                var connectionString = ConnectionStringResolver.Get();
                if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_DB_USER"))
                {
                    throw new Exception("Oracle database connection string is not configured properly.");
                }

                using var dbContext = AppDbContextFactory.Create(connectionString);

                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddTicks(-1);

                var selectedItem = cmbSymbol.SelectedItem as StockFilterItem;
                int selectedStockId = selectedItem?.Id ?? 0;

                // 1. Query EodHistory table
                var historyQuery = dbContext.EodHistory.AsNoTracking().AsQueryable();
                if (selectedStockId > 0)
                {
                    historyQuery = historyQuery.Where(h => h.Id == selectedStockId);
                }
                historyQuery = historyQuery.Where(h => h.Date >= fromDate && h.Date <= toDate);
                var historyList = await historyQuery.ToListAsync();

                // 2. Query EodDaily table (latest records)
                var dailyQuery = dbContext.EodDaily.AsNoTracking().AsQueryable();
                if (selectedStockId > 0)
                {
                    dailyQuery = dailyQuery.Where(d => d.Id == selectedStockId);
                }
                dailyQuery = dailyQuery.Where(d => d.Date >= fromDate && d.Date <= toDate);
                var dailyList = await dailyQuery.ToListAsync();

                // 3. Merge results without duplicates (keyed by Id + Date)
                var recordMap = new Dictionary<(int Id, DateTime Date), EodData>();

                foreach (var h in historyList)
                {
                    recordMap[(h.Id, h.Date.Date)] = h;
                }

                foreach (var d in dailyList)
                {
                    if (!recordMap.ContainsKey((d.Id, d.Date.Date)))
                    {
                        recordMap[(d.Id, d.Date.Date)] = d;
                    }
                }

                var combinedResults = recordMap.Values
                    .OrderByDescending(r => r.Date)
                    .ThenBy(r => r.Id)
                    .ToList();

                bool isCapped = combinedResults.Count > MaxDisplayRecords;
                var displayList = isCapped ? combinedResults.Take(MaxDisplayRecords).ToList() : combinedResults;

                PopulateGridAndStats(displayList, combinedResults.Count, isCapped);

                if (isCapped)
                {
                    SetStatus(success: true, $"✔ Showing top {MaxDisplayRecords:N0} of {combinedResults.Count:N0} records found (narrow date range to see all).");
                }
                else
                {
                    SetStatus(success: true, $"✔ Found {combinedResults.Count:N0} historical record(s).");
                }
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

        private void PopulateGridAndStats(List<EodData> records, int totalCount, bool isCapped)
        {
            dgvHistory.Rows.Clear();
            lblTotalRecords.Text = isCapped
                ? $"Records: {records.Count:N0} of {totalCount:N0}"
                : $"Records: {records.Count:N0}";

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
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            using var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            using var archive = new System.IO.Compression.ZipArchive(fs, System.IO.Compression.ZipArchiveMode.Create);

            // 1. [Content_Types].xml
            var entryContentTypes = archive.CreateEntry("[Content_Types].xml");
            using (var writer = new System.IO.StreamWriter(entryContentTypes.Open(), System.Text.Encoding.UTF8))
            {
                writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                             "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                             "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                             "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                             "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                             "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                             "</Types>");
            }

            // 2. _rels/.rels
            var entryRels = archive.CreateEntry("_rels/.rels");
            using (var writer = new System.IO.StreamWriter(entryRels.Open(), System.Text.Encoding.UTF8))
            {
                writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                             "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                             "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
                             "</Relationships>");
            }

            // 3. xl/_rels/workbook.xml.rels
            var entryWbRels = archive.CreateEntry("xl/_rels/workbook.xml.rels");
            using (var writer = new System.IO.StreamWriter(entryWbRels.Open(), System.Text.Encoding.UTF8))
            {
                writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                             "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                             "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                             "</Relationships>");
            }

            // 4. xl/workbook.xml
            var entryWorkbook = archive.CreateEntry("xl/workbook.xml");
            using (var writer = new System.IO.StreamWriter(entryWorkbook.Open(), System.Text.Encoding.UTF8))
            {
                writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                             "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                             "<sheets><sheet name=\"EOD Historical Data\" sheetId=\"1\" r:id=\"rId1\"/></sheets>" +
                             "</workbook>");
            }

            // 5. xl/worksheets/sheet1.xml
            var entrySheet = archive.CreateEntry("xl/worksheets/sheet1.xml");
            using (var writer = new System.IO.StreamWriter(entrySheet.Open(), System.Text.Encoding.UTF8))
            {
                var sb = new System.Text.StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");
                sb.Append("<sheetData>");

                // Header Row
                sb.Append("<row r=\"1\">");
                int colIdx = 1;
                foreach (DataGridViewColumn col in dgvHistory.Columns)
                {
                    var colLetter = GetExcelColumnName(colIdx++);
                    var title = System.Security.SecurityElement.Escape(col.HeaderText ?? "");
                    sb.Append($"<c r=\"{colLetter}1\" t=\"inlineStr\"><is><t>{title}</t></is></c>");
                }
                sb.Append("</row>");

                // Data Rows
                int rowIdx = 2;
                foreach (DataGridViewRow row in dgvHistory.Rows)
                {
                    if (row.IsNewRow) continue;
                    sb.Append($"<row r=\"{rowIdx}\">");
                    colIdx = 1;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        var colLetter = GetExcelColumnName(colIdx++);
                        var val = System.Security.SecurityElement.Escape(cell.Value?.ToString() ?? "");
                        sb.Append($"<c r=\"{colLetter}{rowIdx}\" t=\"inlineStr\"><is><t>{val}</t></is></c>");
                    }
                    sb.Append("</row>");
                    rowIdx++;
                }

                sb.Append("</sheetData>");
                sb.Append("</worksheet>");

                writer.Write(sb.ToString());
            }
        }

        private static string GetExcelColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = string.Empty;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
        }

        private void ExportToCsv(string filePath)
        {
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
                    pd.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(35, 35, 35, 35);

                    int rowIdx = 0;
                    int pageNumber = 1;

                    pd.PrintPage += (s, e) =>
                    {
                        var g = e.Graphics;
                        if (g == null) return;

                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                        int leftMargin = e.MarginBounds.Left;
                        int topMargin = e.MarginBounds.Top;
                        int printableWidth = e.MarginBounds.Width;
                        int pageHeight = e.MarginBounds.Bottom;

                        using var fontTitle = new Font("Segoe UI", 15, FontStyle.Bold);
                        using var fontHeader = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                        using var fontBody = new Font("Segoe UI", 9f, FontStyle.Regular);
                        using var fontSub = new Font("Segoe UI", 8.5f, FontStyle.Italic);
                        using var fontFooter = new Font("Segoe UI", 8f, FontStyle.Regular);

                        using var brushNavy = new SolidBrush(Color.FromArgb(30, 58, 138));
                        using var brushAltRow = new SolidBrush(Color.FromArgb(245, 247, 250));
                        using var penGrid = new Pen(Color.FromArgb(218, 224, 233));
                        using var penBorder = new Pen(Color.FromArgb(148, 163, 184));

                        // 1. Draw Professional Header Banner
                        int bannerHeight = 44;
                        g.FillRectangle(brushNavy, leftMargin, topMargin, printableWidth, bannerHeight);
                        g.DrawString("EOD Historical Market Data Report", fontTitle, Brushes.White, leftMargin + 15, topMargin + 6);
                        using var brushSub = new SolidBrush(Color.FromArgb(191, 219, 254));
                        g.DrawString($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}  │  Total Records: {dgvHistory.Rows.Count:N0}", fontSub, brushSub, leftMargin + 16, topMargin + 26);

                        // 2. Define Proportional Column Ratios (Sum = 1.0)
                        float[] colRatios = { 0.06f, 0.26f, 0.11f, 0.09f, 0.09f, 0.09f, 0.09f, 0.09f, 0.12f };
                        StringAlignment[] colAlignments = {
                            StringAlignment.Near,   // Stock ID
                            StringAlignment.Near,   // Stock Name
                            StringAlignment.Center, // Date
                            StringAlignment.Far,    // Open
                            StringAlignment.Far,    // High
                            StringAlignment.Far,    // Low
                            StringAlignment.Far,    // Close
                            StringAlignment.Far,    // Adj Close
                            StringAlignment.Far     // Volume
                        };

                        float[] colWidths = new float[colRatios.Length];
                        float[] colXPos = new float[colRatios.Length];
                        float currentX = leftMargin;

                        for (int i = 0; i < colRatios.Length; i++)
                        {
                            colWidths[i] = printableWidth * colRatios[i];
                            colXPos[i] = currentX;
                            currentX += colWidths[i];
                        }

                        // 3. Draw Table Header Row (Dark navy background with bold white text)
                        int y = topMargin + bannerHeight + 12;
                        int headerHeight = 28;
                        int dataRowHeight = 28;

                        g.FillRectangle(brushNavy, leftMargin, y, printableWidth, headerHeight);

                        using var penHeaderDivider = new Pen(Color.FromArgb(59, 130, 246));

                        for (int c = 0; c < dgvHistory.Columns.Count && c < colRatios.Length; c++)
                        {
                            using var format = new StringFormat
                            {
                                Alignment = colAlignments[c],
                                LineAlignment = StringAlignment.Center,
                                Trimming = StringTrimming.EllipsisCharacter
                            };
                            var rect = new RectangleF(colXPos[c] + 6, y, colWidths[c] - 12, headerHeight);
                            g.DrawString(dgvHistory.Columns[c].HeaderText, fontHeader, Brushes.White, rect, format);

                            // Draw vertical column divider in header
                            if (c > 0)
                            {
                                g.DrawLine(penHeaderDivider, colXPos[c], y, colXPos[c], y + headerHeight);
                            }
                        }

                        int tableStartY = y;
                        y += headerHeight;

                        // 4. Draw Data Rows with Zebra Striping & Vertical Gridlines
                        while (rowIdx < dgvHistory.Rows.Count)
                        {
                            var row = dgvHistory.Rows[rowIdx];
                            if (!row.IsNewRow)
                            {
                                // Alternating Row Fill
                                if (rowIdx % 2 == 1)
                                {
                                    g.FillRectangle(brushAltRow, leftMargin, y, printableWidth, dataRowHeight);
                                }
                                else
                                {
                                    g.FillRectangle(Brushes.White, leftMargin, y, printableWidth, dataRowHeight);
                                }

                                // Cell Contents & Vertical Gridlines
                                for (int c = 0; c < row.Cells.Count && c < colRatios.Length; c++)
                                {
                                    var txt = row.Cells[c].Value?.ToString() ?? "-";
                                    using var format = new StringFormat
                                    {
                                        Alignment = colAlignments[c],
                                        LineAlignment = StringAlignment.Center,
                                        Trimming = StringTrimming.EllipsisCharacter
                                    };
                                    var rect = new RectangleF(colXPos[c] + 6, y, colWidths[c] - 12, dataRowHeight);
                                    g.DrawString(txt, fontBody, Brushes.Black, rect, format);

                                    // Vertical Gridline between columns
                                    if (c > 0)
                                    {
                                        g.DrawLine(penGrid, colXPos[c], y, colXPos[c], y + dataRowHeight);
                                    }
                                }

                                // Horizontal Bottom Gridline
                                g.DrawLine(penGrid, leftMargin, y + dataRowHeight, leftMargin + printableWidth, y + dataRowHeight);
                                y += dataRowHeight;
                            }

                            rowIdx++;

                            // Page Break Check
                            if (y + dataRowHeight + 35 > pageHeight && rowIdx < dgvHistory.Rows.Count)
                            {
                                // Draw Outer Frame for the table on this page
                                g.DrawRectangle(penBorder, leftMargin, tableStartY, printableWidth, y - tableStartY);

                                DrawFooter(g, fontFooter, penGrid, leftMargin, pageHeight, printableWidth, pageNumber++);
                                e.HasMorePages = true;
                                return;
                            }
                        }

                        // Draw Outer Table Border Frame
                        g.DrawRectangle(penBorder, leftMargin, tableStartY, printableWidth, y - tableStartY);

                        // Draw Final Page Footer
                        DrawFooter(g, fontFooter, penGrid, leftMargin, pageHeight, printableWidth, pageNumber);
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

        private static void DrawFooter(Graphics g, Font font, Pen pen, int left, int bottom, int width, int pageNum)
        {
            g.DrawLine(pen, left, bottom - 20, left + width, bottom - 20);
            g.DrawString("CONFIDENTIAL  │  TICKR EOD Financial Data Service", font, Brushes.Gray, left, bottom - 16);
            var pageStr = $"Page {pageNum}";
            var size = g.MeasureString(pageStr, font);
            g.DrawString(pageStr, font, Brushes.Gray, left + width - size.Width, bottom - 16);
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

        private void SetStatus(bool success, string message)
        {
            lblHistoryStatus.ForeColor = success
                ? Color.FromArgb(22, 163, 74)   // Green
                : Color.FromArgb(185, 28, 28);  // Red
            lblHistoryStatus.Text = message;
        }
    }
}
