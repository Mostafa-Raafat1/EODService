using System;
using Microsoft.Extensions.Configuration;
using EODService.Config;

namespace EODService.DTOs.SymbolSettings
{
    public class SymbolSettingsMapper
    {
        public static SymbolSettings? MapToSymbolSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(PathsConfig.AppSettingsFileName, optional: false, reloadOnChange: true);

            var activeProviderPath = PathsConfig.ActiveProviderSettingsPath;
            if (!string.IsNullOrWhiteSpace(activeProviderPath))
            {
                builder.AddJsonFile(activeProviderPath, optional: true, reloadOnChange: true);
            }

            var configuration = builder.Build();

            return configuration
                .GetSection("SymbolSettings")
                .Get<SymbolSettings>();
        }
    }
}
