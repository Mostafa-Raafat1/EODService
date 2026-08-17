using System.Collections.Generic;

namespace EODSettingsApp.AppSettingsConfig
{
    /// <summary>
    /// Yahoo Finance provider settings section from AppSettings.json.
    /// </summary>
    public class YahooSettingsSection
    {
        public int    ID       { get; set; } = 1;
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
        public int    ID         { get; set; } = 2;
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
    /// Automated schedule settings section from AppSettings.json.
    /// </summary>
    public class ScheduleSettingsSection
    {
        public bool Enabled { get; set; } = true;
        public List<string> WorkingDays { get; set; } = new() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        public string RunTime { get; set; } = "18:00:00";
    }

    /// <summary>
    /// Connection strings section from AppSettings.json.
    /// </summary>
    public class ConnectionStringsSection
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the provider-specific, schedule, and connection sections of EODService's AppSettings.json.
    /// </summary>
    public class AppSettingsModel
    {
        public YahooSettingsSection      YahooSettings      { get; set; } = new();
        public TwelveDataSettingsSection TwelveDataSettings { get; set; } = new();
        public SymbolSettingsSection     SymbolSettings     { get; set; } = new();
        public ScheduleSettingsSection   ScheduleSettings   { get; set; } = new();
        public ConnectionStringsSection  ConnectionStrings  { get; set; } = new();
    }
}
