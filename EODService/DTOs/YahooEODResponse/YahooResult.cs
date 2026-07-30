using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.YahooEODResponse
{
    public class YahooResult
    {
        public List<long>? Timestamp { get; set; }

        public YahooIndicators? Indicators { get; set; }
    }
}
