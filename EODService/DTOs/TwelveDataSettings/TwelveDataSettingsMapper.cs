using Microsoft.Extensions.Configuration;

namespace EODService.DTOs.TwelveDataSettings
{
    public class TwelveDataSettingsMapper
    {
        public static TwelveDataSettings? MapToTwelveDataSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                // External file overrides the internal one if it exists.
                // This is written by the EODSettingsApp WinForms tool.
                .AddJsonFile(@"C:\EODConfig\settings.json", optional: true, reloadOnChange: true)
                .Build();

            return configuration
                .GetSection("TwelveDataSettings")
                .Get<TwelveDataSettings>();
        }
    }
}
