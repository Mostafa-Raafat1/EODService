using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EODService.Models.Provider;

namespace EODService.Persistance.Repo
{
    public class ProviderRepo : IProvider
    {
        private readonly AppDbContext dbContext;

        public ProviderRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Provider>?> GetAllProvidersAsync()
        {
            var providers = await dbContext.Provider
                .FromSqlInterpolated($@"
            SELECT ID, PROVIDER, API_KEY, BASE_URL, ENDPOINT, PARAMETERS
            FROM PROVIDER")
                .ToListAsync();

            return providers;
        }

        public async Task<Provider?> GetProviderByIdAsync(int providerId)
        {
            var providers = await dbContext.Provider
                .FromSqlInterpolated($@"
                        SELECT ID, PROVIDER, API_KEY, BASE_URL, ENDPOINT, PARAMETERS
                        FROM PROVIDER
                        WHERE ID = {providerId}")
                .ToListAsync();

            return providers.SingleOrDefault();
        }

        public async Task UpdateProvider(int providerId, string name, string baseUrl, string endPoint, string? apiKey, string? parameters)
        {
            var connection = dbContext.Database.GetDbConnection();
            bool closeOnExit = false;

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
                closeOnExit = true;
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE PROVIDER SET PROVIDER = :name, BASE_URL = :baseUrl, ENDPOINT = :endPoint, API_KEY = :apiKey, PARAMETERS = :parameters WHERE ID = :providerId";

                var pName = command.CreateParameter();
                pName.ParameterName = "name";
                pName.Value = (object?)name ?? DBNull.Value;
                command.Parameters.Add(pName);

                var pBaseUrl = command.CreateParameter();
                pBaseUrl.ParameterName = "baseUrl";
                pBaseUrl.Value = (object?)baseUrl ?? DBNull.Value;
                command.Parameters.Add(pBaseUrl);

                var pEndPoint = command.CreateParameter();
                pEndPoint.ParameterName = "endPoint";
                pEndPoint.Value = (object?)endPoint ?? DBNull.Value;
                command.Parameters.Add(pEndPoint);

                var pApiKey = command.CreateParameter();
                pApiKey.ParameterName = "apiKey";
                pApiKey.Value = (object?)apiKey ?? DBNull.Value;
                command.Parameters.Add(pApiKey);

                var pParameters = command.CreateParameter();
                pParameters.ParameterName = "parameters";
                pParameters.Value = (object?)parameters ?? DBNull.Value;
                command.Parameters.Add(pParameters);

                var pId = command.CreateParameter();
                pId.ParameterName = "providerId";
                pId.Value = providerId;
                command.Parameters.Add(pId);

                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (closeOnExit && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }
    }
}
