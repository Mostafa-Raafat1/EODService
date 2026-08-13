using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.YahooSettings
{
    public class YahooSettings
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
    }
}
