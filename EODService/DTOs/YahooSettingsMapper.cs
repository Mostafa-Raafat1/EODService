using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs
{
    public  class YahooSettingsMapper
    {
        public YahooSettings MapToYahooSettings()
        {
            var configuration = new ConfigurationBuilder()
                .
                .AddJsonFile("appsettings.json")
                .Build();
            return new YahooSettings(baseUrl, endpoint, interval, range);
        }
    }
}
