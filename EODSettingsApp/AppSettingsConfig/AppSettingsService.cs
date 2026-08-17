using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EODSettingsApp.AppSettingsConfig
{
    /// <summary>
    /// Reads and writes the Yahoo, TwelveData, and SymbolSettings sections of EODService's AppSettings.json.
    /// Every other section (ProviderSettings, ConnectionStrings, …) is preserved
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
        /// Loads the Yahoo, TwelveData, and SymbolSettings sections from AppSettings.json.
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
                TwelveDataSettings = ReadSection<TwelveDataSettingsSection>(root, "TwelveDataSettings"),
                SymbolSettings     = ReadSection<SymbolSettingsSection>(root, "SymbolSettings"),
                ScheduleSettings   = ReadSection<ScheduleSettingsSection>(root, "ScheduleSettings"),
                ConnectionStrings  = ReadSection<ConnectionStringsSection>(root, "ConnectionStrings")
            };
        }

        /// <summary>
        /// Convenience method: merges only the DefaultConnection string into AppSettings.json
        /// without touching any other section.
        /// </summary>
        public static void SaveConnectionString(string connectionString)
        {
            var mainPath = AppSettingsPath.Resolve();
            var existing = File.ReadAllText(mainPath);
            var root     = JsonNode.Parse(existing)!.AsObject();

            var section = new ConnectionStringsSection { DefaultConnection = connectionString };
            root["ConnectionStrings"] = JsonNode.Parse(JsonSerializer.Serialize(section, _writeOptions));

            var updatedJson = root.ToJsonString(_writeOptions);
            File.WriteAllText(mainPath, updatedJson);

            SyncToCopy(Path.Combine(AppContext.BaseDirectory, AppSettingsPath.FileName), mainPath, updatedJson);
            SyncToSourceProjectFile(mainPath, updatedJson);

            var mainDir = Path.GetDirectoryName(mainPath);
            if (!string.IsNullOrEmpty(mainDir))
            {
                var devBinPath = Path.GetFullPath(Path.Combine(mainDir, "bin", "Debug", "net10.0", AppSettingsPath.FileName));
                SyncToCopy(devBinPath, mainPath, updatedJson);
            }
        }

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
            {
                var section = new ConnectionStringsSection { DefaultConnection = model.ConnectionStrings.DefaultConnection };
                root["ConnectionStrings"] = JsonNode.Parse(JsonSerializer.Serialize(section, _writeOptions));
            }

            var updatedJson = root.ToJsonString(_writeOptions);

            // 1. Primary write
            File.WriteAllText(mainPath, updatedJson);

            // 2. Sync to current application bin directory if distinct
            SyncToCopy(Path.Combine(AppContext.BaseDirectory, AppSettingsPath.FileName), mainPath, updatedJson);

            // 3. Sync back to source EODService project file so rebuild doesn't overwrite values
            SyncToSourceProjectFile(mainPath, updatedJson);

            // 4. Sync to EODService bin/Debug output if distinct and exists
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
        /// Walks up from the currently resolved AppSettings.json path to find and update
        /// the canonical source copy in the EODService project folder.
        /// This prevents a Visual Studio rebuild from overwriting the encrypted bin copy
        /// with a stale plain-text source file.
        /// </summary>
        private static void SyncToSourceProjectFile(string currentPath, string jsonContent)
        {
            try
            {
                var fileName = AppSettingsPath.FileName;
                var dir      = new DirectoryInfo(Path.GetDirectoryName(currentPath) ?? string.Empty);

                while (dir != null)
                {
                    // Look for EODService\AppSettings.json + EODService\Program.cs
                    var subDir       = Path.Combine(dir.FullName, "EODService");
                    var candidate    = Path.Combine(subDir, fileName);
                    var programCs    = Path.Combine(subDir, "Program.cs");

                    if (File.Exists(candidate) && File.Exists(programCs) &&
                        !string.Equals(Path.GetFullPath(candidate), Path.GetFullPath(currentPath), StringComparison.OrdinalIgnoreCase))
                    {
                        File.WriteAllText(candidate, jsonContent);
                        System.Diagnostics.Trace.WriteLine($"[AppSettingsService] Synced encrypted settings to source: {candidate}");
                        return;
                    }

                    dir = dir.Parent;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[AppSettingsService] SyncToSourceProjectFile failed: {ex.Message}");
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
