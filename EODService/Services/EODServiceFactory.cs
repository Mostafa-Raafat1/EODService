using EODService.DTOs.Provider;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.YahooSettings;
using EODService.Models;
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
            int ProviderId,
            SymbolSettings symbolSettings,
            HttpClient httpClient,
            ILoggerFactory loggerFactory,
            ProviderDTO provider)
        {
            switch (provider.Id)
            {
                case (int)ProviderIds.Yahoo:
                    if (provider == null)
                        throw new ArgumentNullException(nameof(provider),
                            "Provider settings must be provided when using the Yahoo provider.");

                    return new YahooEODService(
                        provider,
                        symbolSettings,
                        httpClient,
                        loggerFactory.CreateLogger<YahooEODService>());

                case (int)ProviderIds.TwelveData:
                    if (provider == null)
                        throw new ArgumentNullException(nameof(provider),
                            "Provider settings must be provided when using the TwelveData provider.");

                    return new TwelveDataEODService(
                        provider,
                        symbolSettings,
                        httpClient,
                        loggerFactory.CreateLogger<TwelveDataEODService>());

                default:
                    throw new ArgumentException(
                        $"The provider '{provider.Id}' is not supported. " +
                        $"Check 'ProviderSettings.ActiveProvider' in appsettings.json. " +
                        $"Supported values: 'Yahoo', 'TwelveData'.");
            }
        }
    }
}
