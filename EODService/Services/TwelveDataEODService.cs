using EODService.DTOs.EOD;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.TwelveDataResponse;
using EODService.DTOs.TwelveDataSettings;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EODService.Services
{
    public class TwelveDataEODService : IEODService
    {
        private readonly TwelveDataSettings _twelveDataSettings;
        private readonly SymbolSettings _symbolSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TwelveDataEODService> _logger;

        // Reusable deserialization options — case-insensitive to be safe
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public TwelveDataEODService(
            TwelveDataSettings twelveDataSettings,
            SymbolSettings symbolSettings,
            HttpClient httpClient,
            ILogger<TwelveDataEODService> logger)
        {
            _twelveDataSettings = twelveDataSettings;
            _symbolSettings = symbolSettings;
            _httpClient         = httpClient;
            _logger             = logger;
        }

        /// <inheritdoc />
        public async Task<List<EodData>> GetEodDataAsync()
        {
            var results = new List<EodData>();

            _logger.LogInformation(
                "EOD import started via Twelve Data. Processing {Count} symbol(s).",
                _symbolSettings.Symbols.Count);

            for (int i = 0; i < _symbolSettings.Symbols.Count; i++)
            {
                var symbol = _symbolSettings.Symbols[i];
                var Id = _symbolSettings.Ids[i];
                var name = _symbolSettings.Names[i];
                try
                {
                    _logger.LogInformation("Downloading EOD data for {Symbol} via Twelve Data...", symbol);

                    var url      = BuildUrl(symbol);
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync();

                    // Stream JSON directly to the deserializer to avoid loading the entire response into memory
                    var twelveDataResponse = await JsonSerializer.DeserializeAsync<TwelveDataResponse>(stream, _jsonOptions);

                    if (twelveDataResponse == null)
                    {
                        _logger.LogWarning("Empty or unreadable response for {Symbol}. Skipping.", symbol);
                        continue;
                    }

                    // Delegate mapping to the Twelve Data mapper
                    var eodData = TwelveDataMapper.Map(twelveDataResponse, Id, name);

                    if (eodData != null)
                    {
                        results.Add(eodData);
                        _logger.LogInformation("Successfully downloaded 1 record for {Symbol}.", symbol);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "No valid EOD data found in response for {Symbol}. Skipping.",
                            symbol);
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Symbol not found on Twelve Data — log and skip
                    _logger.LogWarning(
                        "Symbol {Symbol} not found on Twelve Data (404). Skipping.",
                        symbol);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex,
                        "HTTP error while downloading data for {Symbol} via Twelve Data. Skipping.",
                        symbol);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogError(ex,
                        "Request timed out for {Symbol} via Twelve Data. Skipping.",
                        symbol);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Unexpected error while processing {Symbol} via Twelve Data. Skipping.",
                        symbol);
                }

                // Respect Twelve Data rate limit — wait 0.2s between requests
                await Task.Delay(200);
            }

            _logger.LogInformation(
                "EOD import complete via Twelve Data. Total records collected: {Count}.",
                results.Count);

            return results;
        }

        private string BuildUrl(string symbol)
        {
            // Twelve Data uses the base symbol directly (no exchange suffix needed).
            // The exchange is resolved by the API from the symbol itself.
            return $"{_twelveDataSettings.BaseUrl}{_twelveDataSettings.Endpoint}" +
                   $"?symbol={symbol}" +
                   $"&interval={_twelveDataSettings.Interval}" +
                   $"&outputsize={_twelveDataSettings.OutputSize}" +
                   $"&apikey={_twelveDataSettings.ApiKey}";
        }
    }
}
