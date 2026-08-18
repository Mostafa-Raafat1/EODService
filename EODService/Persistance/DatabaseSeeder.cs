using EODService.Models;
using EODService.Models.Provider;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EODService.Persistance
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext dbContext)
        {
            var connection = dbContext.Database.GetDbConnection();

            await connection.OpenAsync();

            try
            {
                // Check Yahoo
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT COUNT(*) FROM PROVIDER WHERE ID = 1";

                    var count = Convert.ToInt32(
                        await command.ExecuteScalarAsync());

                    if (count == 0)
                    {
                        command.CommandText = @"
                    INSERT INTO PROVIDER
                    (
                        ID,
                        PROVIDER,
                        API_KEY,
                        BASE_URL,
                        ENDPOINT,
                        PARAMETERS
                    )
                    VALUES
                    (
                        1,
                        'Yahoo',
                        NULL,
                        'https://query1.finance.yahoo.com',
                        '/v8/finance/chart',
                        '{""Interval"":""1d"",""Range"":""3d""}'
                    )";

                        await command.ExecuteNonQueryAsync();
                    }
                }

                // Check TwelveData
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT COUNT(*) FROM PROVIDER WHERE ID = 2";

                    var count = Convert.ToInt32(
                        await command.ExecuteScalarAsync());

                    if (count == 0)
                    {
                        command.CommandText = @"
                    INSERT INTO PROVIDER
                    (
                        ID,
                        PROVIDER,
                        API_KEY,
                        BASE_URL,
                        ENDPOINT,
                        PARAMETERS
                    )
                    VALUES
                    (
                        2,
                        'TwelveData',
                        '21c6115364964470ac04579771a25555',
                        'https://api.twelvedata.com',
                        '/time_series',
                        '{""Interval"":""1day"",""OutputSize"":5}'
                    )";

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}
