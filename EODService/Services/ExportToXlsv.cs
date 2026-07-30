using ClosedXML.Excel;
using EODService.DTOs.EOD;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Services
{
    public class ExportToXlsv : IExport
    {
        public void Export(List<EodData> eodDataList, string filePath)
        {
            using var workbook = File.Exists(filePath)
            ? new XLWorkbook(filePath)
            : new XLWorkbook();

            var worksheet = workbook.Worksheets.Count == 0
                ? workbook.Worksheets.Add("EOD")
                : workbook.Worksheet("EOD");

            // Create headers if the worksheet is empty
            if (worksheet.LastRowUsed() == null)
            {
                worksheet.Cell(1, 1).Value = "Symbol";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Open";
                worksheet.Cell(1, 4).Value = "High";
                worksheet.Cell(1, 5).Value = "Low";
                worksheet.Cell(1, 6).Value = "Close";
                worksheet.Cell(1, 7).Value = "AdjustedClose";
                worksheet.Cell(1, 8).Value = "Volume";
            }

            // Find the next empty row
            int nextRow = (worksheet.LastRowUsed()?.RowNumber() ?? 1) + 1;

            // Write all records
            foreach (var item in eodDataList)
            {
                worksheet.Cell(nextRow, 1).Value = item.Symbol;
                worksheet.Cell(nextRow, 2).Value = item.Date;
                worksheet.Cell(nextRow, 3).Value = item.Open;
                worksheet.Cell(nextRow, 4).Value = item.High;
                worksheet.Cell(nextRow, 5).Value = item.Low;
                worksheet.Cell(nextRow, 6).Value = item.Close;
                worksheet.Cell(nextRow, 7).Value = item.AdjustedClose;
                worksheet.Cell(nextRow, 8).Value = item.Volume;

                nextRow++;
            }

            worksheet.Columns().AdjustToContents();

            // Save once
            workbook.SaveAs(filePath);
        }
    }
    
}
