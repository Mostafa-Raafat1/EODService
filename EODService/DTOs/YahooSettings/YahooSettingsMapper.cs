using System;
using Microsoft.Extensions.Configuration;
using EODService.Config;
using EODService.Persistance.Repo;
using EODService.Persistance;
using System.Data.Entity.Infrastructure.Design;

namespace EODService.DTOs.YahooSettings
{
    public class YahooSettingsMapper
    {
        public YahooSettingsMapper() { }
        public static async Task<YahooSettings?> MapToYahooSettings(AppDbContext dbContext)
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
            // get the provider from the database
            IProvider provderRepo = new ProviderRepo(dbContext);

            var provider =  await provderRepo.GetProviderById(YahooSettingsDTO.ID);

            if (provider != null)
            {
                YahooSettingsDTO.Name = provider.Name;
                YahooSettingsDTO.BaseUrl = provider.BaseUrl;
                YahooSettingsDTO.Endpoint = provider.EndPoint;
            }

            return YahooSettingsDTO;
        }
    }
}
