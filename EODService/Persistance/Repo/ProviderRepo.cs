using EODService.Models.Provider;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;using System.Text;

namespace EODService.Persistance.Repo
{
    public class ProviderRepo : IProvider
    {
        private readonly AppDbContext dbContext;

        public ProviderRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        // Oracle generate wrong sql for this query .FirstOrDefault(), so we have to use FromSqlInterpolated
        public async Task<List<Provider>?> GetAllProvidersAsync()
        {
            return await dbContext.Provider
                .FromSqlInterpolated($@"
            SELECT ID, PROVIDER, API_KEY, BASE_URL, ENDPOINT, PARAMETERS
            FROM PROVIDER")
                .ToListAsync();
        }

        public async Task<Provider?> GetProviderByIdAsync(int providerId)
        {
            var provider = await dbContext.Provider
                .FromSqlInterpolated($@"
                        SELECT ID, PROVIDER, API_KEY, BASE_URL, ENDPOINT, PARAMETERS
                        FROM PROVIDER
                        WHERE ID = {providerId}")
                .ToListAsync();

            return provider.SingleOrDefault();
        }


        public Task UpdateProvider(int providerId, string name, string baseUrl, string endPoint, string? apiKey)
        {
            throw new NotImplementedException();
        }
    }
}
