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
        public async Task<Provider?> GetProviderById(int providerId)
        {
            var providers = await dbContext.Provider
                .FromSqlInterpolated($@"
            SELECT ID, PROVIDER, API_KEY, BASE_URL, ENDPOINT
            FROM PROVIDER
            WHERE ID = {providerId}")
                .ToListAsync();

            return providers.SingleOrDefault();
        }

        public async Task UpdateProvider(int providerId, string name, string baseUrl, string endPoint, string? apiKey)
        {
            await dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE PROVIDER
                SET PROVIDER = {name},
                    BASE_URL = {baseUrl},
                    ENDPOINT = {endPoint},
                    API_KEY = {apiKey}
                WHERE ID = {providerId}");
        }
    }
}
