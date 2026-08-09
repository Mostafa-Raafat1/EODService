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
                // External file overrides the internal one if it exists.
                // This is written by the EODSettingsApp WinForms tool.
                .AddJsonFile(@"C:\EODConfig\settings.json", optional: true, reloadOnChange: true)
                .Build();

            return configuration
                .GetSection("ProviderSettings")
                .Get<ProviderSettings>();
        }
    }
}
