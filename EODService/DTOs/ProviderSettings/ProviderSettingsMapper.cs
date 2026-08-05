using Microsoft.Extensions.Configuration;

namespace EODService.DTOs.ProviderSettings
{
    public class ProviderSettingsMapper
    {
        public static ProviderSettings? MapToProviderSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return configuration
                .GetSection("ProviderSettings")
                .Get<ProviderSettings>();
        }
    }
}
