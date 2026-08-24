using System;
using System.IO;
using System.Text.Json;

namespace EODService.Config
{
    public class PathesConfigData
    {
        public string ActiveProviderSettingsPath { get; set; } = "EODConfig/settings.json";
        public string AppSettingsPath { get; set; } = "AppSettings.json";
        /// <summary>Root folder where all log sub-folders (by month) and daily log files will be created.</summary>
        public string LogFolderPath { get; set; } = "EODConfig/Logs";
        public string EODServicePath { get; set; } = "EODService.exe";
    }

    /// <summary>
    /// Configurable paths reader — loads file paths from PathsConfig.json or PathesConfig.json so they are not hardcoded.
    /// Supports relative paths that resolve inside the application's base directory.
    /// </summary>
    public static class PathsConfig
    {
        private const string ConfigFileName = "PathesConfig.json";
        private static PathesConfigData? _cachedData;

        public static PathesConfigData Current => LoadConfig();

        public static string ActiveProviderSettingsPath => ResolveFullPath(Current.ActiveProviderSettingsPath);

        public static string ActiveProviderFolderPath =>
            Path.GetDirectoryName(ActiveProviderSettingsPath) ?? Path.Combine(AppContext.BaseDirectory, "EODConfig");

        public static string AppSettingsFileName => Current.AppSettingsPath;

        public static string LogFolderPath => ResolveFullPath(Current.LogFolderPath);

        public static string EODServicePath
        {
            get
            {
                var raw = Current.EODServicePath;
                if (string.IsNullOrWhiteSpace(raw))
                    return string.Empty;

                return ResolveFullPath(raw);
            }
        }

        private static string GetCanonicalBaseDirectory()
        {
            var baseDir = AppContext.BaseDirectory;

            // In development, if running inside bin/Debug/... check if EODSettingsApp exists in solution
            var current = new DirectoryInfo(baseDir);
            while (current != null)
            {
                var settingsAppDir = Path.Combine(current.FullName, "EODSettingsApp");
                if (Directory.Exists(settingsAppDir) && File.Exists(Path.Combine(settingsAppDir, "EODSettingsApp.csproj")))
                {
                    return settingsAppDir;
                }
                current = current.Parent;
            }

            return baseDir;
        }

        private static string ResolveFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return GetCanonicalBaseDirectory();

            if (Path.IsPathRooted(path))
                return path;

            var localPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
            if (File.Exists(localPath) || Directory.Exists(localPath))
                return localPath;

            return Path.GetFullPath(Path.Combine(GetCanonicalBaseDirectory(), path));
        }

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
            EnsureDirectoriesExist();
            return _cachedData;
        }

        /// <summary>
        /// Ensures that the external config folder, logs folder, and default settings file exist on the machine.
        /// Guarantees seamless start on fresh machines without manual folder creation.
        /// </summary>
        public static void EnsureDirectoriesExist()
        {
            try
            {
                var folder = ActiveProviderFolderPath;
                if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var logFolder = LogFolderPath;
                if (!string.IsNullOrWhiteSpace(logFolder) && !Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                var settingsPath = ActiveProviderSettingsPath;
                if (!string.IsNullOrWhiteSpace(settingsPath) && !File.Exists(settingsPath))
                {
                    var defaultSettingsJson = "{\r\n  \"ProviderSettings\": {\r\n    \"ActiveProvider\": 2\r\n  }\r\n}";
                    File.WriteAllText(settingsPath, defaultSettingsJson);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[PathsConfig] EnsureDirectoriesExist error: {ex.Message}");
            }
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
        public static string EODServicePath => PathsConfig.EODServicePath;
    }
}
