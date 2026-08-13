using EODService.Config;
using EODService.Persistance;
using EODService.Persistance.Repo;
using Microsoft.Extensions.Configuration;
using System;

namespace EODService.DTOs.TwelveDataSettings
{
    public class TwelveDataSettingsMapper
    {
        public static async Task<TwelveDataSettings?> MapToTwelveDataSettings(AppDbContext dbContext)
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

            var TwelveDataDTO =  configuration
                .GetSection("TwelveDataSettings")
                .Get<TwelveDataSettings>();

            IProvider provderRepo = new ProviderRepo(dbContext);

            var provider = provderRepo.GetProviderById(TwelveDataDTO.ID).Result;

            if(provider != null) {
                TwelveDataDTO.Name = provider.Name;
                TwelveDataDTO.BaseUrl = provider.BaseUrl;
                TwelveDataDTO.Endpoint = provider.EndPoint;
                TwelveDataDTO.ApiKey = provider.ApiKey;
            }

            return TwelveDataDTO;
        }
    }
}
