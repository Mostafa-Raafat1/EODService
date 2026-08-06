using EODService.DTOs.SymbolSettings;
using EODService.DTOs.TwelveDataSettings;
using EODService.DTOs.YahooSettings;
using Microsoft.Extensions.Logging;

namespace EODService.Services
{
    /// <summary>
    /// Factory responsible for creating the correct IEODService implementation
    /// based on the ActiveProvider configured in appsettings.json.
    /// 
    /// To add a new provider in the future:
    ///   1. Create a new class implementing IEODService (e.g., BloombergEODService)
    ///   2. Add a new case below
    ///   3. Update appsettings.json "ActiveProvider" to the new provider name
    /// </summary>
    public static class EODServiceFactory
    {
        public static IEODService CreateProvider(
            string providerName,
            SymbolSettings symbolSettings,
            HttpClient httpClient,
            ILoggerFactory loggerFactory,
            YahooSettings? yahooSettings = null,
            TwelveDataSettings? twelveDataSettings = null)
        {
            switch (providerName.ToLower())
            {
                case "yahoo":
                    if (yahooSettings == null)
                        throw new ArgumentNullException(nameof(yahooSettings),
                            "YahooSettings must be provided when using the Yahoo provider.");

                    return new YahooEODService(
                        yahooSettings,
                        symbolSettings,
                        httpClient,
                        loggerFactory.CreateLogger<YahooEODService>());

                case "twelvedata":
                    if (twelveDataSettings == null)
                        throw new ArgumentNullException(nameof(twelveDataSettings),
                            "TwelveDataSettings must be provided when using the TwelveData provider.");

                    return new TwelveDataEODService(
                        twelveDataSettings,
                        symbolSettings,
                        httpClient,
                        loggerFactory.CreateLogger<TwelveDataEODService>());

                default:
                    throw new ArgumentException(
                        $"The provider '{providerName}' is not supported. " +
                        $"Check 'ProviderSettings.ActiveProvider' in appsettings.json. " +
                        $"Supported values: 'Yahoo', 'TwelveData'.");
            }
        }
    }
}
