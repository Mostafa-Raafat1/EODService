using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using EODService.Config;
using EODService.Services;

namespace EODService.DTOs.OracleSettings
{
    public static class OracleSettingsMapper
    {
        /// <summary>
        /// Reads the DefaultConnection string from AppSettings.json.
        /// </summary>
        public static string? GetConnectionString(ILogger? logger = null)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(PathesConfig.AppSettingsFileName, optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                logger?.LogError("'ConnectionStrings:DefaultConnection' is missing or empty in {File}.", PathesConfig.AppSettingsFileName);
                return null;
            }

            return connectionString;
        }
    }
}
