using System;
using System.IO;
using System.Text.Json;

namespace EODService.Config
{
    public class PathesConfigData
    {
        public string ActiveProviderSettingsPath { get; set; } = @"C:\EODConfig\settings.json";
        public string AppSettingsPath { get; set; } = "AppSettings.json";
    }

    /// <summary>
    /// Configurable paths reader — loads file paths from PathesConfig.json so they are not hardcoded.
    /// </summary>
    public static class PathesConfig
    {
        private const string ConfigFileName = "PathesConfig.json";
        private static PathesConfigData? _cachedData;

        public static PathesConfigData Current => LoadConfig();

        public static string ActiveProviderSettingsPath => Current.ActiveProviderSettingsPath;

        public static string ActiveProviderFolderPath =>
            Path.GetDirectoryName(ActiveProviderSettingsPath) ?? @"C:\EODConfig";

        public static string AppSettingsFileName => Current.AppSettingsPath;

        public static PathesConfigData LoadConfig()
        {
            if (_cachedData != null)
                return _cachedData;

            try
            {
                var filePath = ResolveConfigFilePath();
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    _cachedData = JsonSerializer.Deserialize<PathesConfigData>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[PathesConfig] Warning: Failed to load {ConfigFileName}, falling back to defaults. Error: {ex.Message}");
            }

            _cachedData ??= new PathesConfigData();
            return _cachedData;
        }

        public static void Reload()
        {
            _cachedData = null;
            LoadConfig();
        }

        private static string ResolveConfigFilePath()
        {
            var baseDir = AppContext.BaseDirectory;
            var localPath = Path.Combine(baseDir, ConfigFileName);
            if (File.Exists(localPath))
                return localPath;

            // Search up directory tree if running in dev environment
            var current = new DirectoryInfo(baseDir);
            while (current != null)
            {
                var fileInCurrent = Path.Combine(current.FullName, ConfigFileName);
                if (File.Exists(fileInCurrent))
                    return fileInCurrent;

                var fileInSubFolder = Path.Combine(current.FullName, "EODService", ConfigFileName);
                if (File.Exists(fileInSubFolder))
                    return fileInSubFolder;

                current = current.Parent;
            }

            return localPath;
        }
    }
}
