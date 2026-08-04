using EODService.DTOs.SymbolSettings;
using EODService.DTOs.YahooSettings;
using EODService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;


var yahooSettings = YahooSettingsMapper.MapToYahooSettings();

if (yahooSettings == null)
{
    Console.WriteLine("ERROR: Could not load YahooSettings from appsettings.json. Exiting.");
    return;
}


// Step 2: Load SymbolSettings from appsettings.json

var symbolSettings = SymbolSettingsMapper.MapToSymbolSettings();

if (symbolSettings == null)
{
    Console.WriteLine("ERROR: Could not load SymbolSettings from appsettings.json. Exiting.");
    return;
}


// Step 3: Create dependencies
using var loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Information));

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add(
    "User-Agent",
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
httpClient.DefaultRequestHeaders.Add(
    "Accept",
    "application/json");

var logger = loggerFactory.CreateLogger<YahooEODService>();


// Step 4: Create the service

IEODService service = new YahooEODService(yahooSettings, symbolSettings, httpClient, logger);


// Step 5: Run the import

Console.WriteLine("Starting EOD data import...\n");

var results = await service.GetEodDataAsync();


// Step 6: Save to Oracle Database
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

        foreach (var result in results)
        {
            var existingRecords = await dbContext.EodDaily
                .Where(e => e.Symbol == result.Symbol && e.Date == result.Date)
                .ToListAsync();
                
            var existingRecord = existingRecords.FirstOrDefault();

            if (existingRecord == null)
            {
                dbContext.EodDaily.Add(result);
            }
            else
            {
                // Update existing record
                existingRecord.Open = result.Open;
                existingRecord.High = result.High;
                existingRecord.Low = result.Low;
                existingRecord.Close = result.Close;
                existingRecord.AdjustedClose = result.AdjustedClose;
                existingRecord.Volume = result.Volume;
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
