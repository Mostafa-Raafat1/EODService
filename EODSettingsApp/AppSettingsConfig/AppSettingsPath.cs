using System;
using System.IO;
using EODService.Config;

namespace EODSettingsApp.AppSettingsConfig
{
    /// <summary>
    /// Single source of truth for locating AppSettings.json.
    /// Prefers the local application running directory, and falls back to
    /// searching parent project folders in development environments.
    /// </summary>
    public static class AppSettingsPath
    {
        private const string EodServiceProject = "EODService";

        public static string FileName => PathsConfig.AppSettingsFileName;

        /// <summary>
        /// Returns the absolute path to the active AppSettings.json file.
        /// </summary>
        public static string Resolve()
        {
            var fileName = FileName;

            // 1. Direct local file in current running directory (Highest Priority)
            var localPath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(localPath))
            {
                return localPath;
            }

            // 2. Search parent folders if running in development environment
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                var fileInCurrent = Path.Combine(current.FullName, fileName);
                var programCsInCurrent = Path.Combine(current.FullName, "Program.cs");
                var csprojInCurrent = Path.Combine(current.FullName, $"{EodServiceProject}.csproj");

                if (File.Exists(fileInCurrent) && (File.Exists(programCsInCurrent) || File.Exists(csprojInCurrent)))
                {
                    return fileInCurrent;
                }

                var projectFolder = Path.Combine(current.FullName, EodServiceProject);
                var fileInSubFolder = Path.Combine(projectFolder, fileName);
                var programCsInSubFolder = Path.Combine(projectFolder, "Program.cs");

                if (File.Exists(fileInSubFolder) && File.Exists(programCsInSubFolder))
                {
                    return fileInSubFolder;
                }

                current = current.Parent;
            }

            return localPath;
        }
    }
}
