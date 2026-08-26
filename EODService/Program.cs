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
using System.Threading;
using System.Threading.Tasks;

// ─── Step 0: Enforce Single Instance Execution ───────────────────────────────
using var mutex = new Mutex(true, @"Global\EODService_Instance_Mutex", out bool isNewInstance);
if (!isNewInstance)
{
    Console.WriteLine("[EODService] Another instance of EODService is already running. Exiting.");
    return;
}

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

if (string.IsNullOrWhiteSpace(connectionString))
{
    // Detailed reason already logged inside OracleSettingsMapper.GetConnectionString()
    Environment.ExitCode = 1;
    return;
}

logger.LogInformation("Connecting to Database...");
AppDbContext dbContext;
try
{
    dbContext = AppDbContextFactory.Create(connectionString);
}
catch (Exception ex)
{
    logger.LogError(ex, "Error occurred while connecting to the database.");
    Environment.ExitCode = 1;
    return;
}

using (dbContext)
{
    // ─── Step 1: Load ProviderSettings ───────────────────────────────────────────
    var providerSettings = ProviderSettingsMapper.MapToProviderSettings();

    if (providerSettings == null || providerSettings.ActiveProvider <= 0)
    {
        logger.LogError("'ProviderSettings:ActiveProvider' is missing or invalid in settings. Exiting.");
        Environment.ExitCode = 1;
        return;
    }

    logger.LogInformation("Active Provider: {Provider}", providerSettings.ActiveProvider);

    // ─── Step 2: Load SymbolSettings ─────────────────────────────────────────────
    var symbolSettings = new SymbolSettings();

    // ─── Step 3: Load Provider data from db ──────────────────────────────────
    IProvider ProviderRepo = new ProviderRepo(dbContext);
    var providers = await ProviderRepo.GetProviderByIdAsync(providerSettings.ActiveProvider);

    // ─── Step 4: Map the Active Provider Data to Settings ──────────────────────────────────

    var ProviderDTO = ProviderMapper.Map(providers);


    switch (providerSettings.ActiveProvider)
    {
        case (int)ProviderIds.Yahoo:

            if (ProviderDTO == null ||
                string.IsNullOrWhiteSpace(ProviderDTO.BaseUrl) ||
                string.IsNullOrWhiteSpace(ProviderDTO.EndPoint))
            {
                logger.LogError(
                    "Yahoo provider config (BaseUrl or Endpoint) could not be loaded. " +
                    "Ensure a row with ID={ProviderId} exists in the PROVIDER table.",
                    ProviderDTO?.Id);

                Environment.ExitCode = 1;
                return;
            }

            symbolSettings = await EodPersistenceService.GetSymbols(
                dbContext,
                s => s.YahooFinanceExists && s.YahooFinanceID != null,
                s => s.YahooFinanceID
            ) ?? new SymbolSettings();

            break;


        case (int)ProviderIds.TwelveData:

            if (ProviderDTO == null ||
                string.IsNullOrWhiteSpace(ProviderDTO.BaseUrl) ||
                string.IsNullOrWhiteSpace(ProviderDTO.EndPoint))
            {
                logger.LogError(
                    "TwelveData provider config (BaseUrl or Endpoint) could not be loaded. " +
                    "Ensure a row with ID={ProviderId} exists in the PROVIDER table.",
                    ProviderDTO?.Id);

                Environment.ExitCode = 1;
                return;
            }

            symbolSettings = await EodPersistenceService.GetSymbols(
                dbContext,
                s => s.TwelveDataExists && s.TwelveDataID != null,
                s => s.TwelveDataID
            ) ?? new SymbolSettings();

            break;


        case (int)ProviderIds.Reuters:

            if (ProviderDTO == null ||
                string.IsNullOrWhiteSpace(ProviderDTO.BaseUrl) ||
                string.IsNullOrWhiteSpace(ProviderDTO.EndPoint))
            {
                logger.LogError(
                    "Reuters provider config (BaseUrl or Endpoint) could not be loaded. " +
                    "Ensure a row with ID={ProviderId} exists in the PROVIDER table.",
                    ProviderDTO?.Id);

                Environment.ExitCode = 1;
                return;
            }

            symbolSettings = await EodPersistenceService.GetSymbols(
                dbContext,
                s => s.ReuterExists && s.ReuterID != null,
                s => s.ReuterID
            ) ?? new SymbolSettings();

            break;


        default:

            logger.LogError(
                "Unsupported provider ID: {ProviderId}",
                providerSettings.ActiveProvider);

            Environment.ExitCode = 1;
            return;
    }

    foreach (var symbol in symbolSettings.Symbols)
    {
        logger.LogInformation("Processing symbol: {Symbol}", symbol);
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
            symbolSettings: symbolSettings,
            httpClient: httpClient,
            loggerFactory: loggerFactory,
            provider: ProviderDTO!);
    }
    catch (ArgumentException ex)
    {
        logger.LogError(ex, "Failed to initialize provider service.");
        Environment.ExitCode = 1;
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
        await EodPersistenceService.SaveEodDataAsync(results, dbContext, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while processing database save operation.");
        Environment.ExitCode = 1;
    }
}

// EODService finished execution successfully
