using System.Collections.Generic;

namespace EODSettingsApp.AppSettingsConfig
{
    /// <summary>
    /// Yahoo Finance provider settings section from AppSettings.json.
    /// </summary>
    public class YahooSettingsSection
    {
        public string BaseUrl  { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public string Range    { get; set; } = string.Empty;
    }

    /// <summary>
    /// Twelve Data provider settings section from AppSettings.json.
    /// </summary>
    public class TwelveDataSettingsSection
    {
        public string BaseUrl    { get; set; } = string.Empty;
        public string Endpoint   { get; set; } = string.Empty;
        public string Interval   { get; set; } = string.Empty;
        public int    OutputSize { get; set; }
        public string ApiKey     { get; set; } = string.Empty;
    }

    /// <summary>
    /// Symbol settings section from AppSettings.json.
    /// </summary>
    public class SymbolSettingsSection
    {
        public List<string> Symbols { get; set; } = new();
    }

    /// <summary>
    /// Represents the editable configuration sections of EODService's AppSettings.json.
    /// </summary>
    public class AppSettingsModel
    {
        public YahooSettingsSection      YahooSettings      { get; set; } = new();
        public TwelveDataSettingsSection TwelveDataSettings { get; set; } = new();
        public SymbolSettingsSection     SymbolSettings     { get; set; } = new();
    }
}
