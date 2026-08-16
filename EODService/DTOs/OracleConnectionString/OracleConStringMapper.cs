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
        /// Reads the DefaultConnection string from AppSettings.json and decrypts it
        /// if it was stored as a DPAPI-encrypted value (prefixed with "ENC:").
        /// Legacy plain-text connection strings are returned unchanged.
        /// </summary>
        public static string? GetConnectionString(ILogger? logger = null)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(PathesConfig.AppSettingsFileName, optional: false, reloadOnChange: true)
                .Build();

            var raw = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(raw))
            {
                logger?.LogError("'ConnectionStrings:DefaultConnection' is missing or empty in {File}.", PathesConfig.AppSettingsFileName);
                return null;
            }

            // Decrypt if stored as DPAPI cipher text; pass through plain-text values unchanged
            var decrypted = SecurityService.Decrypt(raw);

            if (string.IsNullOrWhiteSpace(decrypted))
            {
                logger?.LogError(
                    "DPAPI decryption returned empty string for DefaultConnection. " +
                    "The stored ENC: blob may have been encrypted on a different machine or user context. " +
                    "Open Database Settings in EODServiceManager and re-save the connection string to re-encrypt it on this machine.");
                return null;
            }

            return decrypted;
        }
    }
}
