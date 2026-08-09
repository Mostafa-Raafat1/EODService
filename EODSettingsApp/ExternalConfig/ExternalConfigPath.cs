using System.IO;
using EODService.Config;

namespace EODSettingsApp.ExternalConfig
{
    /// <summary>
    /// Source of truth for the location of the shared external active provider config file.
    /// Reads location dynamically from PathesConfig.json.
    /// </summary>
    public static class ExternalConfigPath
    {
        public static string FolderPath => PathesConfig.ActiveProviderFolderPath;
        public static string FilePath   => PathesConfig.ActiveProviderSettingsPath;
    }
}
