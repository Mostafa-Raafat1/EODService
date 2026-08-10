using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EODService.DTOs.EOD;
using EODService.Persistance;

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

            await dbContext.Database.EnsureCreatedAsync(ct);

            // Wrap entire read-upsert-insert pipeline in an explicit database transaction
            await using var tx = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var symbols = results.Select(r => r.Symbol).Distinct().ToList();

                // 1. Fetch matching EodDaily rows first (100% compatible SQL translation for Oracle EF Core)
                var dailyRows = await dbContext.EodDaily
                    .Where(e => symbols.Contains(e.Symbol))
                    .ToListAsync(ct);

                // Group in memory to avoid Oracle EF Core LINQ provider GroupBy translation bugs
                var existingDailyDict = dailyRows
                    .GroupBy(e => e.Symbol)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var result in results)
                {
                    if (!existingDailyDict.TryGetValue(result.Symbol, out var existing))
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
                    .Where(e => symbols.Contains(e.Symbol))
                    .Select(e => new { e.Symbol, e.Date })
                    .ToListAsync(ct);

                var lastHistoryDates = historyRows
                    .GroupBy(e => e.Symbol)
                    .ToDictionary(g => g.Key, g => (DateTime?)g.Max(x => x.Date));

                foreach (var result in results)
                {
                    lastHistoryDates.TryGetValue(result.Symbol, out var lastDate);
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
    }
}
