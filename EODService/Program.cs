using EODService.DTOs.ProviderSettings;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.TwelveDataSettings;
using EODService.DTOs.YahooSettings;
using EODService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using EODService.DTOs.EOD;

// ─── Step 1: Load ProviderSettings ───────────────────────────────────────────
var providerSettings = ProviderSettingsMapper.MapToProviderSettings();

if (providerSettings == null || string.IsNullOrWhiteSpace(providerSettings.ActiveProvider))
{
    Console.WriteLine("ERROR: 'ProviderSettings:ActiveProvider' is missing or empty in appsettings.json. Exiting.");
    return;
}

Console.WriteLine($"Active Provider: {providerSettings.ActiveProvider}");

// ─── Step 2: Load SymbolSettings ─────────────────────────────────────────────
var symbolSettings = SymbolSettingsMapper.MapToSymbolSettings();

if (symbolSettings == null || symbolSettings.Symbols == null || !symbolSettings.Symbols.Any())
{
    Console.WriteLine("ERROR: 'SymbolSettings:Symbols' is missing or empty in appsettings.json. Exiting.");
    return;
}

// ─── Step 3: Load Provider-Specific Settings ──────────────────────────────────
var yahooSettings       = YahooSettingsMapper.MapToYahooSettings();
var twelveDataSettings  = TwelveDataSettingsMapper.MapToTwelveDataSettings();

// Validate Yahoo settings if it's the active provider
if (providerSettings.ActiveProvider.Equals("Yahoo", StringComparison.OrdinalIgnoreCase))
{
    if (yahooSettings == null || string.IsNullOrWhiteSpace(yahooSettings.BaseUrl) || string.IsNullOrWhiteSpace(yahooSettings.Endpoint))
    {
        Console.WriteLine("ERROR: 'YahooSettings' (BaseUrl or Endpoint) is missing or empty in appsettings.json. Exiting.");
        return;
    }
}

// Validate TwelveData settings if it's the active provider
if (providerSettings.ActiveProvider.Equals("TwelveData", StringComparison.OrdinalIgnoreCase))
{
    if (twelveDataSettings == null || string.IsNullOrWhiteSpace(twelveDataSettings.BaseUrl) || string.IsNullOrWhiteSpace(twelveDataSettings.ApiKey))
    {
        Console.WriteLine("ERROR: 'TwelveDataSettings' (BaseUrl or ApiKey) is missing or empty in appsettings.json. Exiting.");
        return;
    }
}

// ─── Step 4: Create Shared Dependencies ──────────────────────────────────────
using var loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Information));

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add(
    "User-Agent",
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

// ─── Step 5: Create Service via Factory ──────────────────────────────────────
IEODService service;
try
{
    service = EODServiceFactory.CreateProvider(
        providerName:        providerSettings.ActiveProvider,
        symbolSettings:      symbolSettings,
        httpClient:          httpClient,
        loggerFactory:       loggerFactory,
        yahooSettings:       yahooSettings,
        twelveDataSettings:  twelveDataSettings);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
    return;
}

// ─── Step 6: Run the Import ───────────────────────────────────────────────────
Console.WriteLine("Starting EOD data import...\n");
var results = await service.GetEodDataAsync();

// ─── Step 7: Save to Oracle Database ─────────────────────────────────────────
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("WARNING: Connection string 'DefaultConnection' is not configured in appsettings.json. Skipping database save.");
}
else
{
    Console.WriteLine("Saving to Oracle Database...");
    try
    {
        using var dbContext = EODService.Persistance.AppDbContextFactory.Create(connectionString);

        // Automatically create the table if it doesn't exist
        await dbContext.Database.EnsureCreatedAsync();

        // ── Collect all symbols from the fetched results ──────────────────────
        var symbols = results.Select(r => r.Symbol).ToList();

        // ── FIX 1: Load ALL existing daily records in a SINGLE query ──────────
        // Previously: one SELECT per symbol (N+1 round-trips to Oracle).
        // Now: one SELECT ... WHERE symbol IN (...) for all symbols at once.
        var existingDailyDict = await dbContext.EodDaily
            .Where(e => symbols.Contains(e.Symbol))
            .ToDictionaryAsync(e => e.Symbol);

        // ── Upsert EodDaily ───────────────────────────────────────────────────
        foreach (var result in results)
        {
            existingDailyDict.TryGetValue(result.Symbol, out var existingRecord);

            if (existingRecord == null)
            {
                // No record yet — insert
                dbContext.EodDaily.Add(result.ToDaily());
                continue;
            }

            // Existing record is newer — skip
            if (existingRecord.Date > result.Date)
            {
                Console.WriteLine($"WARNING: Existing record for {result.Symbol} on {existingRecord.Date:yyyy-MM-dd} is newer than incoming data ({result.Date:yyyy-MM-dd}). Skipping update.");
                continue;
            }



            else
            {
                // Update existing record
                existingRecord.Open          = result.Open;
                existingRecord.High          = result.High;
                existingRecord.Low           = result.Low;
                existingRecord.Close         = result.Close;
                existingRecord.AdjustedClose = result.AdjustedClose;
                existingRecord.Volume        = result.Volume;
                existingRecord.Date = result.Date.Date;
            }
        }

        // ── FIX 2: Load last history dates in a SINGLE query ─────────────────
        // Previously: one blocking .Result DB call per symbol (N+1 + deadlock risk).
        // Now: one GROUP BY query returning max date per symbol.
        var lastHistoryDates = await dbContext.EodHistory
            .Where(e => symbols.Contains(e.Symbol))
            .GroupBy(e => e.Symbol)
            .Select(g => new { Symbol = g.Key, LastDate = g.Max(x => x.Date) })
            .ToDictionaryAsync(x => x.Symbol, x => (DateTime?)x.LastDate);

        // ── Insert new EodHistory records ─────────────────────────────────────
        foreach (var result in results)
        {
            lastHistoryDates.TryGetValue(result.Symbol, out var lastDate);

            // FIX 3: use strict < (not <=) to avoid re-inserting same-day records
            if (lastDate == null || lastDate < result.Date)
            {
                dbContext.EodHistory.Add(result.ToHistory());
            }
        }

        await dbContext.SaveChangesAsync();
        Console.WriteLine("Data saved to Oracle database successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR saving to database: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"INNER EXCEPTION: {ex.InnerException.Message}");
            if (ex.InnerException.InnerException != null)
            {
                Console.WriteLine($"DEEP INNER EXCEPTION: {ex.InnerException.InnerException.Message}");
            }
        }
    }
}
