using EODService.DTOs.EOD;
using EODService.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EODService.Services
{
    /// <summary>
    /// Centralized persistence service responsible for saving EOD data into the Oracle database.
    /// Manages explicit database transactions and upserts across EodDaily and EodHistory tables.
    /// </summary>
    public static class EodPersistenceService
    {
        public static async Task SaveEodDataAsync(
            IEnumerable<EodData> results,
            AppDbContext dbContext,
            ILogger? logger = null)
        {
            if (results == null || !results.Any())
            {
                logger?.LogWarning("No EOD data provided for database save.");
                return;
            }

            var resultList = results.ToList();
            var symbols = resultList.Select(r => r.Symbol).Distinct().ToList();

            // Wrap entire load + upsert + save in an explicit database transaction
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // ── 1. Upsert EodDaily ──────────────────────────────────────────────
                var existingDailyDict = await dbContext.EodDaily
                    .Where(e => symbols.Contains(e.Symbol))
                    .ToDictionaryAsync(e => e.Symbol);

                foreach (var result in resultList)
                {
                    if (existingDailyDict.TryGetValue(result.Symbol, out var existingRecord))
                    {
                        if (existingRecord.Date > result.Date)
                        {
                            logger?.LogWarning(
                                "Existing record for {Symbol} on {ExistingDate:yyyy-MM-dd} is newer than incoming data ({IncomingDate:yyyy-MM-dd}). Skipping daily update.",
                                result.Symbol, existingRecord.Date, result.Date);
                            continue;
                        }

                        // Update existing daily record
                        existingRecord.Open          = result.Open;
                        existingRecord.High          = result.High;
                        existingRecord.Low           = result.Low;
                        existingRecord.Close         = result.Close;
                        existingRecord.AdjustedClose = result.AdjustedClose;
                        existingRecord.Volume        = result.Volume;
                        existingRecord.Date          = result.Date.Date;
                    }
                    else
                    {
                        dbContext.EodDaily.Add(result.ToDaily());
                    }
                }

                // ── 2. Insert EodHistory ─────────────────────────────────────────────
                var lastHistoryDates = await dbContext.EodHistory
                    .Where(e => symbols.Contains(e.Symbol))
                    .GroupBy(e => e.Symbol)
                    .Select(g => new { Symbol = g.Key, LastDate = g.Max(x => x.Date) })
                    .ToDictionaryAsync(x => x.Symbol, x => (DateTime?)x.LastDate);

                int historyInsertedCount = 0;
                foreach (var result in resultList)
                {
                    lastHistoryDates.TryGetValue(result.Symbol, out var lastDate);

                    if (lastDate == null || lastDate < result.Date)
                    {
                        dbContext.EodHistory.Add(result.ToHistory());
                        historyInsertedCount++;
                    }
                }

                // ── 3. Commit ────────────────────────────────────────────────────────
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                logger?.LogInformation(
                    "Successfully saved EOD data to Oracle DB. Processed {DailyCount} symbol(s), inserted {HistoryCount} new history record(s).",
                    resultList.Count, historyInsertedCount);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger?.LogError(ex, "Failed to save EOD data to database. Transaction rolled back.");
                throw;
            }
        }
    }
}
