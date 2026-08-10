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
var symbolSettings = SymbolSettingsMapper.MapToSymbolSettings();

if (symbolSettings == null || symbolSettings.Symbols == null || !symbolSettings.Symbols.Any())
{
    logger.LogError("'SymbolSettings:Symbols' is missing or empty in appsettings.json. Exiting.");
    return;
}

// ─── Step 3: Load Provider-Specific Settings ──────────────────────────────────
var yahooSettings      = YahooSettingsMapper.MapToYahooSettings();
var twelveDataSettings = TwelveDataSettingsMapper.MapToTwelveDataSettings();

// Validate Yahoo settings if it's the active provider
if (providerSettings.ActiveProvider.Equals("Yahoo", StringComparison.OrdinalIgnoreCase))
{
    if (yahooSettings == null || string.IsNullOrWhiteSpace(yahooSettings.BaseUrl) || string.IsNullOrWhiteSpace(yahooSettings.Endpoint))
    {
        logger.LogError("'YahooSettings' (BaseUrl or Endpoint) is missing or empty in appsettings.json. Exiting.");
        return;
    }
}

// Validate TwelveData settings if it's the active provider
if (providerSettings.ActiveProvider.Equals("TwelveData", StringComparison.OrdinalIgnoreCase))
{
    if (twelveDataSettings == null || string.IsNullOrWhiteSpace(twelveDataSettings.BaseUrl) || string.IsNullOrWhiteSpace(twelveDataSettings.ApiKey))
    {
        logger.LogError("'TwelveDataSettings' (BaseUrl or ApiKey) is missing or empty in appsettings.json. Exiting.");
        return;
    }
}

// ─── Step 4: Create Shared HttpClient with Timeout ──────────────────────────
using var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(30)
};
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
    logger.LogError(ex, "Failed to initialize provider service.");
    return;
}

// ─── Step 6: Run the Import ───────────────────────────────────────────────────
logger.LogInformation("Starting EOD data import...");
var results = await service.GetEodDataAsync();

if (!results.Any())
{
    logger.LogWarning("No data collected from provider. Exiting.");
    return;
}

// ─── Step 7: Save to Oracle Database via Centralized Persistence Service ────

var connectionString = OracleSettingsMapper.GetConnectionString();

if (string.IsNullOrWhiteSpace(connectionString))
{
    logger.LogWarning("Connection string 'DefaultConnection' is missing or unconfigured in appsettings.json. Skipping database save.");
}
else
{
    logger.LogInformation("Saving to Oracle Database...");
    try
    {
        using var dbContext = AppDbContextFactory.Create(connectionString);
        await dbContext.Database.EnsureCreatedAsync();

        await EodPersistenceService.SaveEodDataAsync(results, dbContext, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while processing database save operation.");
    }
}
