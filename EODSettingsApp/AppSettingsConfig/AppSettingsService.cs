using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EODSettingsApp.AppSettingsConfig
{
    /// <summary>
    /// Reads and writes editable sections (Yahoo, TwelveData, SymbolSettings, ScheduleSettings, ConnectionStrings)
    /// of EODService's AppSettings.json. Every other section (ProviderSettings, …) is preserved as-is.
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
        /// Loads settings model sections from AppSettings.json.
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
                SymbolSettings     = ReadSection<SymbolSettingsSection>(root, "SymbolSettings"),
                ScheduleSettings   = ReadSection<ScheduleSettingsSection>(root, "ScheduleSettings"),
                ConnectionStrings  = ReadSection<ConnectionStringsSection>(root, "ConnectionStrings")
            };
        }

        /// <summary>
        /// Saves settings model sections back into AppSettings.json and syncs changes to output files.
        /// </summary>
        public static void Save(AppSettingsModel model)
        {
            var mainPath = AppSettingsPath.Resolve();
            var existing = File.ReadAllText(mainPath);

            var root = JsonNode.Parse(existing)!.AsObject();

            if (model.YahooSettings != null)
                root["YahooSettings"] = JsonNode.Parse(JsonSerializer.Serialize(model.YahooSettings));
                
            if (model.TwelveDataSettings != null)
                root["TwelveDataSettings"] = JsonNode.Parse(JsonSerializer.Serialize(model.TwelveDataSettings));
                
            if (model.SymbolSettings != null)
                root["SymbolSettings"] = JsonNode.Parse(JsonSerializer.Serialize(model.SymbolSettings));

            if (model.ScheduleSettings != null)
                root["ScheduleSettings"] = JsonNode.Parse(JsonSerializer.Serialize(model.ScheduleSettings));

            if (model.ConnectionStrings != null)
                root["ConnectionStrings"] = JsonNode.Parse(JsonSerializer.Serialize(model.ConnectionStrings));

            var updatedJson = root.ToJsonString(_writeOptions);

            // 1. Primary write to resolved path
            File.WriteAllText(mainPath, updatedJson);

            // 2. Sync to current application bin directory if distinct
            SyncToCopy(Path.Combine(AppContext.BaseDirectory, AppSettingsPath.FileName), mainPath, updatedJson);

            // 3. Sync to EODService source project file beside Program.cs if distinct
            try
            {
                var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
                while (currentDir != null)
                {
                    var sourceProjectFile = Path.Combine(currentDir.FullName, "EODService", AppSettingsPath.FileName);
                    var programCs = Path.Combine(currentDir.FullName, "EODService", "Program.cs");

                    if (File.Exists(sourceProjectFile) && File.Exists(programCs))
                    {
                        SyncToCopy(sourceProjectFile, mainPath, updatedJson);
                        break;
                    }
                    currentDir = currentDir.Parent;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[AppSettingsService] Best effort sync to source project file failed: {ex.Message}");
            }
        }

        // ── Private helpers ──────────────────────────────────────────────────────

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

        private static T ReadSection<T>(JsonElement root, string sectionName) where T : new()
        {
            if (!root.TryGetProperty(sectionName, out var section))
                return new T();

            return JsonSerializer.Deserialize<T>(section.GetRawText(), _readOptions)
                   ?? new T();
        }
    }
}
