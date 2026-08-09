using System;
using Microsoft.Extensions.Configuration;
using EODService.Config;

namespace EODService.DTOs.YahooSettings
{
    public class YahooSettingsMapper
    {
        public static YahooSettings? MapToYahooSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(PathesConfig.AppSettingsFileName, optional: false, reloadOnChange: true);

            var activeProviderPath = PathesConfig.ActiveProviderSettingsPath;
            if (!string.IsNullOrWhiteSpace(activeProviderPath))
            {
                builder.AddJsonFile(activeProviderPath, optional: true, reloadOnChange: true);
            }

            var configuration = builder.Build();

            return configuration
                .GetSection("YahooSettings")
                .Get<YahooSettings>();
        }
    }
}
