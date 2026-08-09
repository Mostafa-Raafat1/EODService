using System.IO;
using Microsoft.Extensions.Configuration;
using EODService.Config;

namespace EODService.DTOs.ProviderSettings
{
    public class ProviderSettingsMapper
    {
        public static ProviderSettings? MapToProviderSettings()
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
                .GetSection("ProviderSettings")
                .Get<ProviderSettings>();
        }
    }
}
