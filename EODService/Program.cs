using EODService.DTOs.OracleSettings;
using EODService.DTOs.ProviderSettings;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.TwelveDataSettings;
using EODService.DTOs.YahooSettings;
using EODService.Persistance;
using EODService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

// ─── Setup Logging ───────────────────────────────────────────────────────────
using var loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole()
           .AddProvider(new EODService.Logging.FileLoggerProvider())
           .SetMinimumLevel(LogLevel.Information));

var logger = loggerFactory.CreateLogger("Program");

// ─── Step 1: Load ProviderSettings ───────────────────────────────────────────
var providerSettings = ProviderSettingsMapper.MapToProviderSettings();

if (providerSettings == null || string.IsNullOrWhiteSpace(providerSettings.ActiveProvider))
{
    logger.LogError("'ProviderSettings:ActiveProvider' is missing or empty in appsettings.json. Exiting.");
    return;
}

logger.LogInformation("Active Provider: {Provider}", providerSettings.ActiveProvider);

// ─── Step 2: Load SymbolSettings ─────────────────────────────────────────────
var symbolSettings = new SymbolSettings();


// ─── Step 3: Load Provider-Specific Settings ──────────────────────────────────
var yahooSettings      = YahooSettingsMapper.MapToYahooSettings();
var twelveDataSettings = TwelveDataSettingsMapper.MapToTwelveDataSettings();


// ─── Step 4: Create Connection and Get Symbols for selected provider ──────────────────────────────────
var connectionString = OracleSettingsMapper.GetConnectionString();
// Create a DbContext instance for database operations
AppDbContext? dbContext = null;

if (string.IsNullOrWhiteSpace(connectionString))
{
    logger.LogWarning("Connection string 'DefaultConnection' is missing or unconfigured in appsettings.json. Skipping database save.");
}
else
{
    logger.LogInformation("Connecting to Database...");
    try
    {
        dbContext = AppDbContextFactory.Create(connectionString);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while connecting to the database.");
        return;
    }
}

// Validate Yahoo settings if it's the active provider and get symbols from database
if (providerSettings.ActiveProvider.Equals("Yahoo", StringComparison.OrdinalIgnoreCase))
{
    if (yahooSettings == null || string.IsNullOrWhiteSpace(yahooSettings.BaseUrl) || string.IsNullOrWhiteSpace(yahooSettings.Endpoint))
    {
        logger.LogError("'YahooSettings' (BaseUrl or Endpoint) is missing or empty in appsettings.json. Exiting.");
        return;
    }
    symbolSettings = await EodPersistenceService.GetSymbolsForYahooFinance(dbContext!) ?? new SymbolSettings();
    foreach(var symbol in symbolSettings.Symbols)
    {
        logger.LogInformation($"Processing symbol: {symbol}");
    }
}

// Validate TwelveData settings if it's the active provider and get symbols from database
if (providerSettings.ActiveProvider.Equals("TwelveData", StringComparison.OrdinalIgnoreCase))
{
    if (twelveDataSettings == null || string.IsNullOrWhiteSpace(twelveDataSettings.BaseUrl) || string.IsNullOrWhiteSpace(twelveDataSettings.ApiKey))
    {
        logger.LogError("'TwelveDataSettings' (BaseUrl or ApiKey) is missing or empty in appsettings.json. Exiting.");
        return;
    }
    symbolSettings = await EodPersistenceService.GetSymbolsForTwelveData(dbContext!) ?? new SymbolSettings();
    foreach (var symbol in symbolSettings.Symbols)
    {
        logger.LogInformation($"Processing symbol: {symbol}");
    }
}


// ─── Step 5: Create Shared HttpClient with Timeout ──────────────────────────
using var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(30)
};
httpClient.DefaultRequestHeaders.Add(
    "User-Agent",
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
httpClient.DefaultRequestHeaders.Add("Accept", "application/json");


// ─── Step 6: Create Service via Factory ──────────────────────────────────────
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
    logger.LogError(ex, "Failed to initialize provider service.");
    return;
}

// ─── Step 7: Run the Import ───────────────────────────────────────────────────
logger.LogInformation("Starting EOD data import...");
var results = await service.GetEodDataAsync();

if (!results.Any())
{
    logger.LogWarning("No data collected from provider. Exiting.");
    return;
}

// ─── Step 8: Save to Oracle Database via Centralized Persistence Service ────

logger.LogInformation("Saving to Oracle Database...");
try
{
    await EodPersistenceService.SaveEodDataAsync(results, dbContext!, logger);
}
catch (Exception ex)
{
        logger.LogError(ex, "Error occurred while processing database save operation.");
}

