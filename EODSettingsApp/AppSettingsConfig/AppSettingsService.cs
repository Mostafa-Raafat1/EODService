using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EODSettingsApp.AppSettingsConfig
{
    /// <summary>
    /// Reads and writes only the Yahoo and TwelveData sections of EODService's AppSettings.json.
    /// Every other section (ProviderSettings, SymbolSettings, ConnectionStrings, …) is preserved
    /// exactly as-is during a save operation.
    /// </summary>
    public static class AppSettingsService
    {
        private static readonly JsonSerializerOptions _readOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonSerializerOptions _writeOptions = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// Loads the Yahoo and TwelveData settings from AppSettings.json.
        /// Returns default (empty) sections if a section key is missing from the file.
        /// </summary>
        public static AppSettingsModel Load()
        {
            var path = AppSettingsPath.Resolve();
            var json = File.ReadAllText(path);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            return new AppSettingsModel
            {
                YahooSettings      = ReadSection<YahooSettingsSection>(root, "YahooSettings"),
                TwelveDataSettings = ReadSection<TwelveDataSettingsSection>(root, "TwelveDataSettings")
            };
        }

        /// <summary>
        /// Saves the Yahoo and TwelveData sections back into AppSettings.json.
        /// All other keys in the file are left untouched.
        /// </summary>
        public static void Save(AppSettingsModel model)
        {
            var path     = AppSettingsPath.Resolve();
            var existing = File.ReadAllText(path);

            var root = JsonNode.Parse(existing)!.AsObject();

            root["YahooSettings"]      = JsonNode.Parse(JsonSerializer.Serialize(model.YahooSettings));
            root["TwelveDataSettings"] = JsonNode.Parse(JsonSerializer.Serialize(model.TwelveDataSettings));

            File.WriteAllText(path, root.ToJsonString(_writeOptions));
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Deserialises a named section from a JSON root element.
        /// Returns a new default instance when the section is absent or malformed.
        /// </summary>
        private static T ReadSection<T>(JsonElement root, string sectionName) where T : new()
        {
            if (!root.TryGetProperty(sectionName, out var section))
                return new T();

            return JsonSerializer.Deserialize<T>(section.GetRawText(), _readOptions)
                   ?? new T();
        }
    }
}
