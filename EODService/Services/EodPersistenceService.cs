using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EODService.Persistance;
using EODService.DTOs.SymbolSettings;
using EODService.Persistance.Repo;
using EODService.DTOs.EOD;
using EODService.Models.Provider;

namespace EODService.Services
{
    /// <summary>
    /// Shared service responsible for atomic, transaction-wrapped database persistence
    /// of EOD market data to Oracle DB. Used by both EODService (console) and EODSettingsApp.
    /// </summary>
    public static class EodPersistenceService
    {
        /// <summary>
        /// Saves fetched EodData records into EodDaily (upsert) and EodHistory (insert new)
        /// within a single atomic database transaction.
        /// </summary>
        public static async Task SaveAsync(AppDbContext dbContext, IEnumerable<EodData> results, CancellationToken ct = default)
        {
            if (results == null || !results.Any())
                return;

            // Wrap entire read-upsert-insert pipeline in an explicit database transaction
            await using var tx = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var symbols = results.Select(r => r.Id).Distinct().ToList();

                // 1. Fetch matching EodDaily rows as NoTracking
                var dailyRows = await dbContext.EodDaily
                    .AsNoTracking()
                    .Where(e => symbols.Contains(e.Id))
                    .ToListAsync(ct);

                var existingDailyIds = dailyRows.Select(e => e.Id).ToHashSet();

                foreach (var result in results)
                {
                    var dailyEntity = result.ToDaily();
                    if (existingDailyIds.Contains(result.Id))
                    {
                        dbContext.EodDaily.Update(dailyEntity);
                    }
                    else
                    {
                        dbContext.EodDaily.Add(dailyEntity);
                    }
                }

                // 2. Fetch history dates as NoTracking
                var historyRows = await dbContext.EodHistory
                    .AsNoTracking()
                    .Where(e => symbols.Contains(e.Id))
                    .Select(e => new { e.Id, e.Date })
                    .ToListAsync(ct);

                var existingHistoryKeys = historyRows
                    .Select(x => (x.Id, x.Date))
                    .ToHashSet();

                foreach (var result in results)
                {
                    if (!existingHistoryKeys.Contains((result.Id, result.Date)))
                    {
                        dbContext.EodHistory.Add(result.ToHistory());
                    }
                }

                // 3. Commit changes and transaction atomically
                await dbContext.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Compatibility overload for logging support.
        /// </summary>
        public static async Task SaveEodDataAsync(IEnumerable<EodData> results, AppDbContext dbContext, ILogger? logger = null, CancellationToken ct = default)
        {
            logger?.LogInformation("Executing atomic database transaction for {Count} record(s)...", results.Count());
            await SaveAsync(dbContext, results, ct);
            logger?.LogInformation("✔ Database save completed successfully.");
        }


        // Later will be enhanced using delegates to be more flexible
        public static async Task<SymbolSettings?> GetSymbolsForYahooFinance(AppDbContext dbContext)
        {
            IStock repo = new StockRepo(dbContext);
            var stocks = await repo.GetSymbolAndTickerIDForYahooFinance();
            return stocks;
        }

        public static async Task<SymbolSettings?> GetSymbolsForTwelveData(AppDbContext dbContext)
        {
            IStock repo = new StockRepo(dbContext);
            var stocks = await repo.GetSymbolAndTickerIDForTwelveData();
            return stocks;
        }

        public static async Task<Provider?> GetProviderById(AppDbContext dbContext, int providerId)
        {
            IProvider repo = new ProviderRepo(dbContext);
            var provider = await repo.GetProviderByIdAsync(providerId);
            return provider;
        }

        public static async Task UpdateProvider(AppDbContext dbContext, int providerId, string name, string baseUrl, string endPoint, string? apiKey)
        {
            IProvider repo = new ProviderRepo(dbContext);
            await repo.UpdateProvider(providerId, name, baseUrl, endPoint, apiKey);
        }
    }
}
