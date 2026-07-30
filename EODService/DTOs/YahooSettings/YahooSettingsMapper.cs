using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.YahooSettings
{
    public  class YahooSettingsMapper
    {
        public static YahooSettings? MapToYahooSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            YahooSettings yahooSettings = new YahooSettings();

            var yahooSettingsSection = configuration
                                        .GetSection("YahooSettings")
                                        .Get<YahooSettings>();

            return yahooSettingsSection;
        }
    }
}
