using EODService.DTOs.EOD;
using EODService.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EODService.Services
{
    /// <summary>
    /// Centralized persistence service responsible for saving EOD data into the Oracle database.
    /// Manages explicit database transactions, handles dictionary duplicate key safety,
    /// and performs atomic upserts across EodDaily and EodHistory tables.
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

            var resultList = results.Where(r => !string.IsNullOrWhiteSpace(r.Symbol)).ToList();
            if (!resultList.Any())
            {
                logger?.LogWarning("All provided EOD records have empty symbols. Aborting save.");
                return;
            }

            var symbols = resultList.Select(r => r.Symbol).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            // Wrap entire load + upsert + save in an explicit database transaction
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            try
            {
                // ── 1. Upsert EodDaily ──────────────────────────────────────────────
                // FIX for ToDictionaryAsync duplicate key exception: Group by Symbol first to prevent duplicate key crashes
                var existingDailyList = await dbContext.EodDaily
                    .Where(e => symbols.Contains(e.Symbol))
                    .ToListAsync();

                var existingDailyDict = existingDailyList
                    .GroupBy(e => e.Symbol, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

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
                        var newDaily = result.ToDaily();
                        dbContext.EodDaily.Add(newDaily);
                        existingDailyDict[result.Symbol] = newDaily; // Track inserted record in dictionary to handle duplicate incoming results
                    }
                }

                // ── 2. Insert EodHistory ─────────────────────────────────────────────
                // FIX for ToDictionaryAsync duplicate key exception: Group by Symbol safely
                var historyGroupList = await dbContext.EodHistory
                    .Where(e => symbols.Contains(e.Symbol))
                    .GroupBy(e => e.Symbol)
                    .Select(g => new { Symbol = g.Key, LastDate = g.Max(x => x.Date) })
                    .ToListAsync();

                var lastHistoryDates = historyGroupList
                    .GroupBy(x => x.Symbol, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => (DateTime?)g.Max(x => x.LastDate), StringComparer.OrdinalIgnoreCase);

                int historyInsertedCount = 0;
                foreach (var result in resultList)
                {
                    lastHistoryDates.TryGetValue(result.Symbol, out var lastDate);

                    if (lastDate == null || lastDate < result.Date)
                    {
                        dbContext.EodHistory.Add(result.ToHistory());
                        lastHistoryDates[result.Symbol] = result.Date; // Track updated max date
                        historyInsertedCount++;
                    }
                }

                // ── 3. Save & Commit ──────────────────────────────────────────────────
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
