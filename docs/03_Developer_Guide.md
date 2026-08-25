# Developer Guide

This document is intended for software engineers maintaining or extending the EODService system.

## Project Structure
- `EODService.sln`: Main solution file.
- `EODService/`: .NET 10.0 Console Application. Responsible for the actual data fetching and database operations.
- `EODSettingsApp/`: .NET 10.0 WinForms Application. The UI for configuration.

## Database Mapping (Entity Framework Core)
The system uses `Oracle.EntityFrameworkCore` to map to the following tables:
1. `EOD_STOCKS` (`Stock` entity): Master list of symbols and per-provider active flags.
2. `PROVIDER` (`Provider` entity): API configurations and JSON parameters.
3. `EodDaily` (`EodDataDaily` entity): Upserted on every run; holds the latest EOD price.
4. `EodHistory` (`EodDataHistory` entity): Insert-only historical log; composite key on `(STOCK_ID, DATE)`.

## Adding a New Data Provider
The system uses a Factory Pattern (`EODServiceFactory`) to initialize providers dynamically based on the Active Provider ID.

To add a new provider (e.g., `Bloomberg`):
1. **Database**: Insert a new row into the `PROVIDER` Oracle table with the next sequential ID (e.g., ID = 4).
2. **Enum**: Add the new provider to `ProviderIds.cs` in the `EODService.Models` namespace.
   ```csharp
   public enum ProviderIds {
       Yahoo = 1, TwelveData = 2, Reuters = 3, Bloomberg = 4
   }
   ```
3. **Implementation**: Create a new class `BloombergEODService` implementing `IEODService`.
4. **Factory**: Update `EODServiceFactory.CreateProvider()` with a new `case (int)ProviderIds.Bloomberg:` to return your instantiated service.
5. **UI**: Update the WinForms `ProviderSettingsForm.cs` to include a configuration tab for the new provider.

## Execution Flow (Console App)
1. Initializes `FileLoggerProvider`.
2. Reads `ActiveProvider` ID from `C:\EODConfig\settings.json`.
3. Loads provider config and active symbols from Oracle DB.
4. `EODServiceFactory` instantiates the active provider.
5. Provider fetches data (REST JSON or WebSocket).
6. Data is passed to `EodPersistenceService.SaveEodDataAsync()` which executes an atomic transaction (Upsert Daily + Insert History).
