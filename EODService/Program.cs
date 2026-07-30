using EODService.DTOs.SymbolSettings;
using EODService.DTOs.YahooSettings;
using EODService.Services;
using Microsoft.Extensions.Logging;


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


// Step 6: Export results to Excel

Console.WriteLine($"Import complete. {results.Count} record(s) collected.\n");

IExport exporter = new ExportToXlsv();
exporter.Export(results, "Stocks.xlsx");

Console.WriteLine("Results exported to Stocks.xlsx successfully.");
