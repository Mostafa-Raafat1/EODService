using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.Stock
{
    public class Stock
    {
        public int Id { get; set; }
        public int SC_Comp_Id { get; set; }
        public string StockName { get; set; } = string.Empty;

        public string? YahooFinanceID { get; set; }
        public string? TwelveDataID { get; set; }
        public string? ReuterID { get; set; }

        public bool YahooFinanceExists { get; set; }
        public bool TwelveDataExists { get; set; }
        public bool ReuterExists { get; set; }

        public string? StockExchange { get; set; }
        public string? ISIN { get; set; }
    }
}
