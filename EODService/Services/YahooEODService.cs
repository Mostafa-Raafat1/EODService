using EODService.DTOs.EOD;
using EODService.DTOs.SymbolSettings;
using EODService.DTOs.YahooSettings;
using Microsoft.Extensions.Logging;

namespace EODService.Services
{
    
    public class YahooEODService : IEODService
    {
        private readonly YahooSettings _yahooSettings;
        private readonly SymbolSettings _symbolSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<YahooEODService> _logger;
        private readonly IYahooResponseMapper _mapper;

        public YahooEODService(
            YahooSettings yahooSettings,
            SymbolSettings symbolSettings,
            HttpClient httpClient,
            ILogger<YahooEODService> logger,
            IYahooResponseMapper mapper)
        {
            _yahooSettings = yahooSettings;
            _symbolSettings = symbolSettings;
            _httpClient = httpClient;
            _logger = logger;
            _mapper = mapper;
        }

       
        public async Task<List<EodDataDto>> GetEodDataAsync()
        {
            var results = new List<EodDataDto>();

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

                    var eodData = _mapper.Map(json, symbol);
                    results.AddRange(eodData);

                    _logger.LogInformation(
                        "Successfully downloaded {Count} record(s) for {Symbol}.",
                        eodData.Count, symbol);
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

       
        /// Build Yahoo Finance URL for our api 
        
        private string BuildUrl(string symbol)
        {
            return $"{_yahooSettings.BaseUrl}{_yahooSettings.Endpoint}{symbol}" +
                   $"?interval={_yahooSettings.Interval}&range={_yahooSettings.Range}";
        }
    }
}
