using EODService.DTOs.OracleSettings;
using EODService.DTOs.Provider;
using EODService.DTOs.ProviderSettings;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.YahooSettings;
using EODService.Models;
using EODService.Persistance;
using EODService.Persistance.Repo;
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

// Ensure runtime folders (C:\EODConfig, C:\EODConfig\Logs) exist on fresh machines
EODService.Config.PathsConfig.EnsureDirectoriesExist();

// Write a banner to the log file so each run is clearly separated
EODService.Logging.FileLoggerProvider.WriteRunBanner();

// ─── Step 4: Create Connection and Get Symbols for selected provider ──────────────────────────────────
var connectionString = OracleSettingsMapper.GetConnectionString(logger);
// Create a DbContext instance for database operations
AppDbContext? dbContext = null;

if (string.IsNullOrWhiteSpace(connectionString))
{
    // Detailed reason already logged inside OracleSettingsMapper.GetConnectionString()
    return;
}

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

// ─── Step 1: Load ProviderSettings ───────────────────────────────────────────
var providerSettings = ProviderSettingsMapper.MapToProviderSettings();

if (providerSettings == null || providerSettings.ActiveProvider <= 0)
{
    logger.LogError("'ProviderSettings:ActiveProvider' is missing or invalid in settings. Exiting.");
    return;
}

logger.LogInformation("Active Provider: {Provider}", providerSettings.ActiveProvider);

// ─── Step 2: Load SymbolSettings ─────────────────────────────────────────────
var symbolSettings = new SymbolSettings();

// ─── Step 3: Load Provider data from db ──────────────────────────────────
IProvider ProviderRepo = new ProviderRepo(dbContext!);
var providers = await ProviderRepo.GetProviderByIdAsync(providerSettings.ActiveProvider);

// ─── Step 4: Map the Active Provider Data to Settings ──────────────────────────────────

var ProviderDTO = ProviderMapper.Map(providers);


// Validate Yahoo settings if it's the active provider and get symbols from database
if (providerSettings.ActiveProvider == (int)ProviderIds.Yahoo)
{
    if (ProviderDTO == null || string.IsNullOrWhiteSpace(ProviderDTO.BaseUrl) || string.IsNullOrWhiteSpace(ProviderDTO.EndPoint))
    {
        logger.LogError("Yahoo provider config (BaseUrl or Endpoint) could not be loaded. Ensure a row with ID={YahooId} exists in the PROVIDER table.", ProviderDTO?.Id);
        return;
    }
    symbolSettings = await EodPersistenceService.GetSymbolsForYahooFinance(dbContext!) ?? new SymbolSettings();
    foreach(var symbol in symbolSettings.Symbols)
    {
        logger.LogInformation($"Processing symbol: {symbol}");
    }
}


// Validate TwelveData settings if it's the active provider and get symbols from database
if (providerSettings.ActiveProvider == (int)ProviderIds.TwelveData)
{
    if (ProviderDTO == null || string.IsNullOrWhiteSpace(ProviderDTO.BaseUrl) || string.IsNullOrWhiteSpace(ProviderDTO.EndPoint))
    {
        logger.LogError("TwelveData provider config (BaseUrl or Endpoint) could not be loaded. Ensure a row with ID={ProviderDTO?.Id} exists in the PROVIDER table.", ProviderDTO?.Id);
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
        symbolSettings:      symbolSettings,
        httpClient:          httpClient,
        loggerFactory:       loggerFactory,
        provider:            ProviderDTO!);
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

