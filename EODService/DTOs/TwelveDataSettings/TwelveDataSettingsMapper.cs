using EODService.Config;
using EODService.Persistance;
using EODService.Persistance.Repo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EODService.DTOs.TwelveDataSettings
{
    public class TwelveDataSettingsMapper
    {
        public static async Task<TwelveDataSettings?> MapToTwelveDataSettings(AppDbContext? dbContext, ILogger? logger = null)
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

            var TwelveDataDTO = configuration
                .GetSection("TwelveDataSettings")
                .Get<TwelveDataSettings>();

            if (TwelveDataDTO == null)
            {
                logger?.LogWarning("TwelveDataSettings section not found in configuration.");
                return null;
            }

            if (dbContext == null)
            {
                logger?.LogWarning("DbContext is null — skipping PROVIDER table lookup for TwelveData (ID={ID}).", TwelveDataDTO.ID);
                return TwelveDataDTO;
            }

            try
            {
                IProvider provderRepo = new ProviderRepo(dbContext);
                var provider = await provderRepo.GetProviderById(TwelveDataDTO.ID);

                if (provider != null)
                {
                    logger?.LogInformation("TwelveData provider loaded from DB: {Name}, BaseUrl={BaseUrl}", provider.Name, provider.BaseUrl);
                    TwelveDataDTO.Name = provider.Name;
                    TwelveDataDTO.BaseUrl = provider.BaseUrl;
                    TwelveDataDTO.Endpoint = provider.EndPoint;
                    TwelveDataDTO.ApiKey = provider.ApiKey;
                }
                else
                {
                    logger?.LogWarning("No row found in PROVIDER table for TwelveData ID={ID}.", TwelveDataDTO.ID);
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to load TwelveData provider config from PROVIDER table (ID={ID}).", TwelveDataDTO.ID);
            }

            return TwelveDataDTO;
        }
    }
}
