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
                .Build();

            return configuration
                .GetSection("TwelveDataSettings")
                .Get<TwelveDataSettings>();
        }
    }
}
