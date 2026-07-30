using EODService.DTOs.EOD;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.YahooEODResponse;
using EODService.DTOs.YahooSettings;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EODService.Services
{
    /// <summary>
    /// Implements IEODService using the Yahoo Finance Chart API as the data source.
    /// Iterates over all configured symbols, fetches raw JSON, deserializes it,
    /// and delegates the mapping to YahooEoadMapper.
    /// </summary>
    public class YahooEODService : IEODService
    {
        private readonly YahooSettings _yahooSettings;
        private readonly SymbolSettings _symbolSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<YahooEODService> _logger;

        // Reusable deserialization options — case-insensitive so "chart" in JSON maps to "Chart" in C#
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public YahooEODService(
            YahooSettings yahooSettings,
            SymbolSettings symbolSettings,
            HttpClient httpClient,
            ILogger<YahooEODService> logger)
        {
            _yahooSettings = yahooSettings;
            _symbolSettings = symbolSettings;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<List<EodData>> GetEodDataAsync()
        {
            var results = new List<EodData>();

            _logger.LogInformation(
                "EOD import started. Processing {Count} symbol(s).",
                _symbolSettings.Symbols.Count);

            foreach (var symbol in _symbolSettings.Symbols)
            {
                try
                {
                    _logger.LogInformation("Downloading EOD data for {Symbol}...", symbol);

                    var url = BuildUrl(symbol);
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    // Deserialize raw JSON into YahooEodResponse DTO
                    var yahooResponse = JsonSerializer.Deserialize<YahooEodResponse>(json, _jsonOptions);

                    if (yahooResponse == null)
                    {
                        _logger.LogWarning("Empty or unreadable response for {Symbol}. Skipping.", symbol);
                        continue;
                    }

                    // Delegate mapping to Mustafa's static mapper
                    var eodData = YahooEoadMapper.Map(yahooResponse, symbol);

                    if (eodData != null)
                    {
                        results.Add(eodData);
                        _logger.LogInformation(
                            "Successfully downloaded 1 record for {Symbol}.",
                            symbol);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "No valid EOD data found in response for {Symbol}. Skipping.",
                            symbol);
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex,
                        "HTTP error while downloading data for {Symbol}. Skipping.",
                        symbol);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogError(ex,
                        "Request timed out for {Symbol}. Skipping.",
                        symbol);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Unexpected error while processing {Symbol}. Skipping.",
                        symbol);
                }
            }

            _logger.LogInformation(
                "EOD import complete. Total records collected: {Count}.",
                results.Count);

            return results;
        }

        /// <summary>
        /// Builds the Yahoo Finance Chart API URL for a given symbol
        /// using the configured base URL, endpoint, interval, and range.
        /// </summary>
        private string BuildUrl(string symbol)
        {
            return $"{_yahooSettings.BaseUrl}{_yahooSettings.Endpoint}{symbol}" +
                   $"?interval={_yahooSettings.Interval}&range={_yahooSettings.Range}";
        }
    }
}
