using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs
{
    public class YahooSettings
    {
        public string BaseUrl { get; set; }
        public string Endpoint{ get; set; }
        public string Interval { get; set; }
        public string Range { get; set; }
        public YahooSettings(string baseUrl, string endpoint, string interval, string range)
        {
            BaseUrl = baseUrl;
            Endpoint = endpoint;
            Interval = interval;
            Range = range;
        }
    }
}
