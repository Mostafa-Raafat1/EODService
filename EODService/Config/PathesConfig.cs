using System;
using System.IO;
using System.Text.Json;

namespace EODService.Config
{
    public class PathesConfigData
    {
        public string ActiveProviderSettingsPath { get; set; } = @"C:\EODConfig\settings.json";
        public string AppSettingsPath { get; set; } = "AppSettings.json";
        /// <summary>Root folder where all log sub-folders (by month) and daily log files will be created.</summary>
        public string LogFolderPath { get; set; } = @"C:\EODConfig\Logs";
    }

    /// <summary>
    /// Configurable paths reader — loads file paths from PathsConfig.json or PathesConfig.json so they are not hardcoded.
    /// </summary>
    public static class PathsConfig
    {
        private const string ConfigFileName = "PathesConfig.json";
        private static PathesConfigData? _cachedData;

        public static PathesConfigData Current => LoadConfig();

        public static string ActiveProviderSettingsPath => Current.ActiveProviderSettingsPath;

        public static string ActiveProviderFolderPath =>
            Path.GetDirectoryName(ActiveProviderSettingsPath) ?? @"C:\EODConfig";

        public static string AppSettingsFileName => Current.AppSettingsPath;

        public static string LogFolderPath => Current.LogFolderPath;

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
                System.Diagnostics.Trace.WriteLine($"[PathsConfig] Warning: Failed to load {ConfigFileName}, falling back to defaults. Error: {ex.Message}");
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

    /// <summary>Backward compatible alias for PathsConfig.</summary>
    [Obsolete("Use PathsConfig instead.")]
    public static class PathesConfig
    {
        public static PathesConfigData Current => PathsConfig.Current;
        public static string ActiveProviderSettingsPath => PathsConfig.ActiveProviderSettingsPath;
        public static string ActiveProviderFolderPath => PathsConfig.ActiveProviderFolderPath;
        public static string AppSettingsFileName => PathsConfig.AppSettingsFileName;
        public static string LogFolderPath => PathsConfig.LogFolderPath;
    }
}
