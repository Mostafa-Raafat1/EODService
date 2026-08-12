using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EODService.DTOs.EOD;
using EODService.Persistance;
using EODService.DTOs.SymbolSettings;
using EODService.Persistance.Repo;

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

                // 1. Fetch matching EodDaily rows first (100% compatible SQL translation for Oracle EF Core)
                var dailyRows = await dbContext.EodDaily
                    .Where(e => symbols.Contains(e.Id))
                    .ToListAsync(ct);

                // Group in memory to avoid Oracle EF Core LINQ provider GroupBy translation bugs
                var existingDailyDict = dailyRows
                    .GroupBy(e => e.Id)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var result in results)
                {
                    if (!existingDailyDict.TryGetValue(result.Id, out var existing))
                    {
                        dbContext.EodDaily.Add(result.ToDaily());
                    }
                    else if (existing.Date <= result.Date)
                    {
                        existing.Date          = result.Date;
                        existing.Open          = result.Open;
                        existing.High          = result.High;
                        existing.Low           = result.Low;
                        existing.Close         = result.Close;
                        existing.AdjustedClose = result.AdjustedClose;
                        existing.Volume        = result.Volume;
                    }
                }

                // 2. Fetch history dates safely
                var historyRows = await dbContext.EodHistory
                    .Where(e => symbols.Contains(e.Id))
                    .Select(e => new { e.Id, e.Date })
                    .ToListAsync(ct);

                var lastHistoryDates = historyRows
                    .GroupBy(e => e.Id)
                    .ToDictionary(g => g.Key, g => (DateTime?)g.Max(x => x.Date));

                foreach (var result in results)
                {
                    lastHistoryDates.TryGetValue(result.Id, out var lastDate);
                    if (lastDate == null || lastDate < result.Date)
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
    }
}
