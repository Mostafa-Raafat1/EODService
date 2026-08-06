using EODService.DTOs.ProviderSettings;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.TwelveDataSettings;
using EODService.DTOs.YahooSettings;
using EODService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using EODService.Persistance.Repo;
using EODService.DTOs.EOD;

// ─── Step 1: Load ProviderSettings ───────────────────────────────────────────
var providerSettings = ProviderSettingsMapper.MapToProviderSettings();

if (providerSettings == null)
{
    Console.WriteLine("ERROR: Could not load ProviderSettings from appsettings.json. Exiting.");
    return;
}

Console.WriteLine($"Active Provider: {providerSettings.ActiveProvider}");

// ─── Step 2: Load SymbolSettings ─────────────────────────────────────────────
var symbolSettings = SymbolSettingsMapper.MapToSymbolSettings();

if (symbolSettings == null)
{
    Console.WriteLine("ERROR: Could not load SymbolSettings from appsettings.json. Exiting.");
    return;
}

// ─── Step 3: Load Provider-Specific Settings ──────────────────────────────────
var yahooSettings       = YahooSettingsMapper.MapToYahooSettings();
var twelveDataSettings  = TwelveDataSettingsMapper.MapToTwelveDataSettings();

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

        // Automatically create the table if it doesn't exist and update columns if altered
        await dbContext.Database.EnsureCreatedAsync();
        // Adding Repo
        var eodRepo = new EodDataRepo(dbContext);

        // Add to daily table, or update if already exists (based on Symbol + Date)
        foreach (var result in results)
        {
            var targetDate    = result.Date.Date;          // e.g. 2026-08-04 00:00:00
            var nextDay       = targetDate.AddDays(1);     // e.g. 2026-08-05 00:00:00

            // Use a date range instead of .Date.Date equality:
            // Oracle's EF Core provider does not reliably translate .Date in a WHERE clause,
            // which caused duplicates when Yahoo (stores 07:00 UTC) and Twelve Data
            // (stores 00:00 UTC) both wrote to the same calendar day.
            // A range query (>= start of day AND < start of next day) works on any database.
            var existingRecords = await dbContext.EodDaily
                .Where(e => e.Symbol == result.Symbol)
                .ToListAsync();

            var existingRecord = existingRecords.FirstOrDefault();

            // if the data is old
            if(existingRecord.Date > result.Date)
            {
                Console.WriteLine($"WARNING: Existing record for {result.Symbol} on {existingRecord.Date:yyyy-MM-dd} is newer than incoming data ({result.Date:yyyy-MM-dd}). Skipping update.");
                continue;
            }


            if (existingRecord == null)
            {
                dbContext.EodDaily.Add(result.ToDaily());
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
            }
        }


        // Add to history table
        DateTime? lastHistoryDate = null;
        foreach (var result in results)
        {
            lastHistoryDate = eodRepo.GetLastDateForSymbol(result.Symbol).Result;
            if (lastHistoryDate == null || lastHistoryDate <= result.Date)
            {
                await dbContext.EodHistory.AddAsync(result.ToHistory());
            }
            continue;
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
