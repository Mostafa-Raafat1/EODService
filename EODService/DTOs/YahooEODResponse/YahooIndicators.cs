using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.YahooEODResponse
{
    public class YahooIndicators
    {
        public List<YahooQuote>? Quote { get; set; }

        public List<YahooAdjClose>? Adjclose { get; set; }
    }
}
