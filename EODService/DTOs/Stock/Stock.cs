using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.Stock
{
    public class Stock
    {
        public int Id { get; set; }

        public string LongEngName { get; set; }
        public string LongArName { get; set; }
        public string ShortEngName { get; set; }
        public string ShortArName { get; set; }

        public string EnglishAddress { get; set; }
        public string ArabicAddress { get; set; }

        public string TickerID { get; set; }
        public string? YahooFinanceID { get; set; }
        public string? TwelveDataID { get; set; }

        public bool YahooFinanceExists { get; set; }
        public bool TwelveDataExists { get; set; }
    }
}
