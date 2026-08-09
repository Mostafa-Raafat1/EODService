using Microsoft.Extensions.Configuration;
using System;
using EODService.Config;

namespace EODService.DTOs.OracleSettings
{
    public static class OracleSettingsMapper
    {
        public static string? GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(PathesConfig.AppSettingsFileName, optional: false, reloadOnChange: true)
                .Build();

            return configuration.GetConnectionString("DefaultConnection");
        }
    }
}
