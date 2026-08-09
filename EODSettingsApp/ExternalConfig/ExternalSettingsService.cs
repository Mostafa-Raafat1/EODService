using System;
using System.IO;
using System.Text.Json;

namespace EODSettingsApp.ExternalConfig
{
    /// <summary>
    /// Handles reading and writing of the external C:\EODConfig\settings.json file.
    /// </summary>
    public static class ExternalSettingsService
    {
        private static readonly JsonSerializerOptions _writeOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        /// <summary>
        /// Loads the external settings file.
        /// Returns default settings (TwelveData) if the file does not yet exist.
        /// </summary>
        public static ExternalSettings Load()
        {
            if (!File.Exists(ExternalConfigPath.FilePath))
                return new ExternalSettings();

            try
            {
                var json = File.ReadAllText(ExternalConfigPath.FilePath);
                return JsonSerializer.Deserialize<ExternalSettings>(json)
                       ?? new ExternalSettings();
            }
            catch
            {
                // If the file is corrupted or unreadable, return safe defaults
                return new ExternalSettings();
            }
        }

        /// <summary>
        /// Saves the provided settings to C:\EODConfig\settings.json.
        /// Creates the folder and file if they do not exist.
        /// </summary>
        public static void Save(ExternalSettings settings)
        {
            // Ensure the folder exists
            Directory.CreateDirectory(ExternalConfigPath.FolderPath);

            var json = JsonSerializer.Serialize(settings, _writeOptions);
            File.WriteAllText(ExternalConfigPath.FilePath, json);
        }
    }
}
