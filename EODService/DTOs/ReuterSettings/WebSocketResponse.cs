using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EODService.DTOs.ReuterSettings
{
    public class WebSocketResponse
    {
        [JsonPropertyName("ID")]
        public int Id { get; set; }

        [JsonPropertyName("Type")]
        public string? Type { get; set; }

        [JsonPropertyName("Key")]
        public WebSocketKey? Key { get; set; }

        [JsonPropertyName("State")]
        public WebSocketState? State { get; set; }

        [JsonPropertyName("Qos")]
        public WebSocketQos? Qos { get; set; }

        [JsonPropertyName("PermData")]
        public string? PermData { get; set; }

        [JsonPropertyName("SeqNumber")]
        public long? SeqNumber { get; set; }

        [JsonPropertyName("Fields")]
        public WebSocketFields? Fields { get; set; }
    }

    public class WebSocketKey
    {
        [JsonPropertyName("Service")]
        public string? Service { get; set; }

        [JsonPropertyName("Name")]
        public string? Name { get; set; }
    }

    public class WebSocketState
    {
        [JsonPropertyName("Stream")]
        public string? Stream { get; set; }

        [JsonPropertyName("Data")]
        public string? Data { get; set; }
    }

    public class WebSocketQos
    {
        [JsonPropertyName("Timeliness")]
        public string? Timeliness { get; set; }

        [JsonPropertyName("Rate")]
        public string? Rate { get; set; }
    }

    public class WebSocketFields
    {
        [JsonPropertyName("HIGH_1")]
        public decimal? High { get; set; }

        [JsonPropertyName("LOW_1")]
        public decimal? Low { get; set; }

        [JsonPropertyName("TRADE_DATE")]
        public string? TradeDate { get; set; }

        [JsonPropertyName("OPEN_PRC")]
        public decimal? Open { get; set; }

        [JsonPropertyName("ACVOL_1")]
        public long? Volume { get; set; }

        [JsonPropertyName("ADJUST_CLS")]
        public decimal? AdjustedClose { get; set; }

        [JsonPropertyName("OFF_CLOSE")]
        public decimal? Close { get; set; }
    }
}
