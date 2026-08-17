using EODService.DTOs.EOD;
using EODService.DTOs.Provider;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.TwelveDataResponse;
using EODService.DTOs.TwelveDataSettings;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EODService.Services
{
    public class TwelveDataEODService : IEODService
    {
        private readonly ProviderDTO _twelveDataSettings;
        private readonly SymbolSettings _symbolSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TwelveDataEODService> _logger;
        private readonly TwelveDataParametersDTO _parameters;

        // Reusable deserialization options — case-insensitive to be safe
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public TwelveDataEODService(
            ProviderDTO twelveDataSettings,
            SymbolSettings symbolSettings,
            HttpClient httpClient,
            ILogger<TwelveDataEODService> logger)
        {
            _twelveDataSettings = twelveDataSettings;
            _symbolSettings = symbolSettings;
            _httpClient         = httpClient;
            _logger             = logger;

            _parameters =
                JsonSerializer.Deserialize<TwelveDataParametersDTO>(
                    _twelveDataSettings.Parameters ?? "{}",
                    _jsonOptions)
                ?? new TwelveDataParametersDTO();
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
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Twelve Data API returned HTTP {StatusCode} for {Symbol}: {ErrorBody}", response.StatusCode, symbol, errorBody);
                        continue;
                    }

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
            var interval = !string.IsNullOrWhiteSpace(_parameters.Interval) ? _parameters.Interval : "1day";
            var outputSize = _parameters.OutputSize > 0 ? _parameters.OutputSize : 1;
            var apiKey = _twelveDataSettings.ApiKey ?? string.Empty;

            return $"{_twelveDataSettings.BaseUrl}{_twelveDataSettings.EndPoint}" +
                   $"?symbol={symbol}" +
                   $"&interval={interval}" +
                   $"&outputsize={outputSize}" +
                   $"&apikey={apiKey}";
        }
    }
}
