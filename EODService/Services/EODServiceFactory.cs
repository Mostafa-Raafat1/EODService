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
            SymbolSettings symbolSettings,
            HttpClient httpClient,
            ILoggerFactory loggerFactory,
            ProviderDTO provider)
        {
            ArgumentNullException.ThrowIfNull(provider,"Provider settings must be provided.");  // one real guard, at the top

            switch (provider.Id)
            {
                case (int)ProviderIds.Yahoo:
                    return new YahooEODService(
                        provider,
                        symbolSettings,
                        httpClient,
                        loggerFactory.CreateLogger<YahooEODService>());

                case (int)ProviderIds.TwelveData:
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
