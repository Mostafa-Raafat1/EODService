using EODService.Models.Provider;
using EODService.Services;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EODService.Persistance.Repo
{
    public class ProviderRepo : IProvider
    {
        private readonly AppDbContext dbContext;

        public ProviderRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Oracle generates wrong sql for this query .FirstOrDefault(), so we use FromSqlInterpolated
        public async Task<Provider?> GetProviderById(int providerId)
        {
            var providers = await dbContext.Provider
                .FromSqlInterpolated($@"
            SELECT ID, PROVIDER, API_KEY, BASE_URL, ENDPOINT
            FROM PROVIDER
            WHERE ID = {providerId}")
                .ToListAsync();

            var provider = providers.SingleOrDefault();

            // Decrypt the API key using AES-256 (shared key across devices)
            if (provider != null && !string.IsNullOrEmpty(provider.ApiKey))
            {
                var aesKey = KeyStoreService.GetKey();
                if (aesKey != null)
                {
                    provider.ApiKey = AesEncryptionService.Decrypt(provider.ApiKey, aesKey);
                }
            }

            return provider;
        }

        public async Task UpdateProvider(int providerId, string name, string baseUrl, string endPoint, string? apiKey)
        {
            // Encrypt the API key using AES-256 with shared key
            string? encryptedApiKey = apiKey;
            if (!string.IsNullOrEmpty(apiKey))
            {
                var aesKey = KeyStoreService.GetKey();
                if (aesKey != null)
                {
                    encryptedApiKey = AesEncryptionService.Encrypt(apiKey, aesKey);
                }
            }

            await dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE PROVIDER
                SET PROVIDER = {name},
                    BASE_URL = {baseUrl},
                    ENDPOINT = {endPoint},
                    API_KEY = {encryptedApiKey}
                WHERE ID = {providerId}");
        }
    }
}
