using Microsoft.Extensions.Configuration;
using System;
using EODService.Config;
using EODService.Services;

namespace EODService.DTOs.OracleSettings
{
    public static class OracleSettingsMapper
    {
        /// <summary>
        /// Reads the DefaultConnection string from AppSettings.json and decrypts it
        /// if it was stored as a DPAPI-encrypted value (prefixed with "ENC:").
        /// Legacy plain-text connection strings are returned unchanged.
        /// </summary>
        public static string? GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(PathesConfig.AppSettingsFileName, optional: false, reloadOnChange: true)
                .Build();

            var raw = configuration.GetConnectionString("DefaultConnection");

            // Decrypt if stored as DPAPI cipher text; pass through plain-text values unchanged
            return SecurityService.Decrypt(raw);
        }
    }
}
