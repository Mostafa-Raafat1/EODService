using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EODSettingsApp.AppSettingsConfig
{
    /// <summary>
    /// Reads and writes Yahoo, TwelveData, and Schedule sections of EODService's AppSettings.json.
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
        /// Loads Yahoo, TwelveData, and Schedule settings from AppSettings.json.
        /// Returns default sections if a section key is missing from the file.
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
                TwelveDataSettings = ReadSection<TwelveDataSettingsSection>(root, "TwelveDataSettings"),
                ScheduleSettings   = ReadSection<ScheduleSettingsSection>(root, "ScheduleSettings")
            };
        }

        /// <summary>
        /// Saves Yahoo, TwelveData, and Schedule sections back into the main EODService AppSettings.json
        /// (beside Program.cs) and syncs changes to active bin output files.
        /// All other keys in the file are left untouched.
        /// </summary>
        public static void Save(AppSettingsModel model)
        {
            var mainPath = AppSettingsPath.Resolve();
            var existing = File.ReadAllText(mainPath);

            var root = JsonNode.Parse(existing)!.AsObject();

            root["YahooSettings"]      = JsonNode.Parse(JsonSerializer.Serialize(model.YahooSettings));
            root["TwelveDataSettings"] = JsonNode.Parse(JsonSerializer.Serialize(model.TwelveDataSettings));
            root["ScheduleSettings"]   = JsonNode.Parse(JsonSerializer.Serialize(model.ScheduleSettings));

            var updatedJson = root.ToJsonString(_writeOptions);

            // 1. Primary write: main EODService AppSettings.json beside Program.cs
            File.WriteAllText(mainPath, updatedJson);

            // 2. Sync to current application bin directory if distinct
            SyncToCopy(Path.Combine(AppContext.BaseDirectory, AppSettingsPath.FileName), mainPath, updatedJson);

            // 3. Sync to EODService bin/Debug output if distinct and exists
            var mainDir = Path.GetDirectoryName(mainPath);
            if (!string.IsNullOrEmpty(mainDir))
            {
                var devBinPath = Path.GetFullPath(Path.Combine(mainDir, "bin", "Debug", "net10.0", AppSettingsPath.FileName));
                SyncToCopy(devBinPath, mainPath, updatedJson);
            }
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Synchronizes updated JSON content to binary output copy if it exists and is distinct from mainPath.
        /// </summary>
        private static void SyncToCopy(string targetPath, string mainPath, string jsonContent)
        {
            try
            {
                if (File.Exists(targetPath) && !string.Equals(Path.GetFullPath(targetPath), Path.GetFullPath(mainPath), StringComparison.OrdinalIgnoreCase))
                {
                    File.WriteAllText(targetPath, jsonContent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[AppSettingsService] Best effort sync to '{targetPath}' failed: {ex.Message}");
            }
        }

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
