using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EODService.Config;
using EODService.Persistance.Repo;
using EODService.Persistance;

namespace EODService.DTOs.YahooSettings
{
    public class YahooSettingsMapper
    {
        public YahooSettingsMapper() { }
        public static async Task<YahooSettings?> MapToYahooSettings(AppDbContext? dbContext, ILogger? logger = null)
        {
            // get configuration of the provider from appsettings
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(PathesConfig.AppSettingsFileName, optional: false, reloadOnChange: true);

            var activeProviderPath = PathesConfig.ActiveProviderSettingsPath;
            if (!string.IsNullOrWhiteSpace(activeProviderPath))
            {
                builder.AddJsonFile(activeProviderPath, optional: true, reloadOnChange: true);
            }

            var configuration = builder.Build();

            var YahooSettingsDTO = configuration
                .GetSection("YahooSettings")
                .Get<YahooSettings>();

            if (YahooSettingsDTO == null)
            {
                logger?.LogWarning("YahooSettings section not found in configuration.");
                return null;
            }

            // get the provider from the database
            if (dbContext == null)
            {
                logger?.LogWarning("DbContext is null — skipping PROVIDER table lookup for Yahoo (ID={ID}).", YahooSettingsDTO.ID);
                return YahooSettingsDTO;
            }

            try
            {
                IProvider provderRepo = new ProviderRepo(dbContext);
                var provider = await provderRepo.GetProviderById(YahooSettingsDTO.ID);

                if (provider != null)
                {
                    logger?.LogInformation("Yahoo provider loaded from DB: {Name}, BaseUrl={BaseUrl}", provider.Name, provider.BaseUrl);
                    YahooSettingsDTO.Name     = provider.Name;
                    YahooSettingsDTO.BaseUrl  = provider.BaseUrl;
                    YahooSettingsDTO.Endpoint = provider.EndPoint;
                }
                else
                {
                    logger?.LogWarning("No row found in PROVIDER table for Yahoo ID={ID}.", YahooSettingsDTO.ID);
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to load Yahoo provider config from PROVIDER table (ID={ID}).", YahooSettingsDTO.ID);
            }

            return YahooSettingsDTO;
        }
    }
}
