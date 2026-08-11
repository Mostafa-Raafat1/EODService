using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.EOD
{
    public class EodData
    {
        public string TickerID { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public decimal? Open { get; set; }

        public decimal? High { get; set; }

        public decimal? Low { get; set; }

        public decimal? Close { get; set; }

        public decimal? AdjustedClose { get; set; }

        public long? Volume { get; set; }
    }
}

