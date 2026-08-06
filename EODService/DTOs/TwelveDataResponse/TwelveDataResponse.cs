using System.Text.Json.Serialization;

namespace EODService.DTOs.TwelveDataResponse
{
    /// <summary>
    /// Root response object from Twelve Data Time Series API.
    /// </summary>
    public class TwelveDataResponse
    {
        [JsonPropertyName("meta")]
        public TwelveDataMeta? Meta { get; set; }

        [JsonPropertyName("values")]
        public List<TwelveDataValue>? Values { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    /// <summary>
    /// Metadata block returned by Twelve Data.
    /// </summary>
    public class TwelveDataMeta
    {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("exchange")]
        public string? Exchange { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    /// <summary>
    /// A single OHLCV candle returned by Twelve Data.
    /// All numeric fields are strings in the API response.
    /// </summary>
    public class TwelveDataValue
    {
        [JsonPropertyName("datetime")]
        public string? Datetime { get; set; }

        [JsonPropertyName("open")]
        public string? Open { get; set; }

        [JsonPropertyName("high")]
        public string? High { get; set; }

        [JsonPropertyName("low")]
        public string? Low { get; set; }

        [JsonPropertyName("close")]
        public string? Close { get; set; }

        [JsonPropertyName("volume")]
        public string? Volume { get; set; }
    }
}
