using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.YahooSettings
{
    public class YahooSettings
    {
        public string BaseUrl { get; set; }
        public string Endpoint{ get; set; }
        public string Interval { get; set; }
        public string Range { get; set; }
    }
}
