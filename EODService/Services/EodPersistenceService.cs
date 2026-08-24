using EODService.DTOs.EOD;
using EODService.DTOs.Stock;
using EODService.DTOs.SymbolSettings;
using EODService.Models;
using EODService.Models.Provider;
using EODService.Persistance;
using EODService.Persistance.Repo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            // Deduplicate and normalize input items to date-only component (00:00:00)
            var cleanResults = results
                .Select(r =>
                {
                    r.Date = r.Date.Date;
                    return r;
                })
                .GroupBy(r => (r.Id, r.Date))
                .Select(g => g.First())
                .ToList();

            // Wrap entire read-upsert-insert pipeline in an explicit database transaction
            await using var tx = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var symbols = cleanResults.Select(r => r.Id).Distinct().ToList();
                var minDate = cleanResults.Min(r => r.Date);
                var maxDate = cleanResults.Max(r => r.Date);

                // 1. Fetch matching EodDaily rows in chunks of 500 to respect Oracle's 1000 IN-clause limit (ORA-01795)
                var existingDailyIds = new HashSet<int>();
                foreach (var chunk in symbols.Chunk(500))
                {
                    var chunkIds = await dbContext.EodDaily
                        .AsNoTracking()
                        .Where(e => chunk.Contains(e.Id))
                        .Select(e => e.Id)
                        .ToListAsync(ct);

                    foreach (var id in chunkIds)
                    {
                        existingDailyIds.Add(id);
                    }
                }

                foreach (var result in cleanResults)
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

                // 2. Fetch existing history rows bounded by minDate & maxDate in chunks of 500
                var existingHistoryKeys = new HashSet<(int Id, DateTime Date)>();
                foreach (var chunk in symbols.Chunk(500))
                {
                    var chunkHistoryRows = await dbContext.EodHistory
                        .AsNoTracking()
                        .Where(e => chunk.Contains(e.Id) && e.Date >= minDate && e.Date <= maxDate)
                        .Select(e => new { e.Id, e.Date })
                        .ToListAsync(ct);

                    foreach (var row in chunkHistoryRows)
                    {
                        existingHistoryKeys.Add((row.Id, row.Date.Date));
                    }
                }

                foreach (var result in cleanResults)
                {
                    var key = (result.Id, result.Date.Date);
                    if (!existingHistoryKeys.Contains(key))
                    {
                        dbContext.EodHistory.Add(result.ToHistory());
                        existingHistoryKeys.Add(key); // Prevent in-batch duplicate insertions
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

        //delegated func
        public static async Task<SymbolSettings?> GetSymbols(
            AppDbContext dbContext,
            Expression<Func<Stock, bool>> existsCondition,
            Func<Stock, string?> tickerSelector)
        {
            IStock repo = new StockRepo(dbContext);

            return await repo.GetSymbolAndTickerIDAsync(
                existsCondition,
                tickerSelector);
        }

        public static async Task<Provider?> GetProviderById(AppDbContext dbContext, int providerId)
        {
            IProvider repo = new ProviderRepo(dbContext);
            var provider = await repo.GetProviderByIdAsync(providerId);
            return provider;
        }



        public static async Task UpdateProvider(AppDbContext dbContext, int providerId, string name, string baseUrl, string endPoint, string? apiKey, string? parameters = null)
        {
            IProvider repo = new ProviderRepo(dbContext);
            await repo.UpdateProvider(providerId, name, baseUrl, endPoint, apiKey, parameters);
        }
    }
}
