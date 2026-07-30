using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.YahooEODResponse
{
    public class YahooQuote
    {
        public List<decimal?>? Open { get; set; }

        public List<decimal?>? High { get; set; }

        public List<decimal?>? Low { get; set; }

        public List<decimal?>? Close { get; set; }

        public List<long?>? Volume { get; set; }
    }
}
