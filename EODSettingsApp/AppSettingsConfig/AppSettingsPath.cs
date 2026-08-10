using System;
using System.IO;
using EODService.Config;

namespace EODSettingsApp.AppSettingsConfig
{
    /// <summary>
    /// Single source of truth for locating EODService's main AppSettings.json file.
    /// Uses PathesConfig.json for file name resolution and dynamically traverses up
    /// the directory tree to find the main project directory beside Program.cs.
    /// </summary>
    public static class AppSettingsPath
    {
        private const string EodServiceProject = "EODService";

        public static string FileName => PathesConfig.AppSettingsFileName;

        /// <summary>
        /// Returns the absolute path to the main AppSettings.json beside Program.cs in the EODService project.
        /// </summary>
        public static string Resolve()
        {
            var fileName = FileName;
            var current = new DirectoryInfo(AppContext.BaseDirectory);

            while (current != null)
            {
                // Option A: We are inside the EODService project folder (or one of its subfolders like bin/Debug/net10.0)
                var fileInCurrent = Path.Combine(current.FullName, fileName);
                var programCsInCurrent = Path.Combine(current.FullName, "Program.cs");
                var csprojInCurrent = Path.Combine(current.FullName, $"{EodServiceProject}.csproj");

                if (File.Exists(fileInCurrent) && (File.Exists(programCsInCurrent) || File.Exists(csprojInCurrent)))
                {
                    return fileInCurrent;
                }

                // Option B: We are in a parent folder (e.g. solution root or EODSettingsApp folder) containing the EODService project subfolder
                var projectFolder = Path.Combine(current.FullName, EodServiceProject);
                var fileInSubFolder = Path.Combine(projectFolder, fileName);
                var programCsInSubFolder = Path.Combine(projectFolder, "Program.cs");

                if (File.Exists(fileInSubFolder) && File.Exists(programCsInSubFolder))
                {
                    return fileInSubFolder;
                }

                current = current.Parent;
            }

            // Standalone / production deployment fallback
            var productionPath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(productionPath))
            {
                return productionPath;
            }

            throw new FileNotFoundException(
                $"Could not locate main '{fileName}' beside Program.cs in the '{EodServiceProject}' project directory.",
                fileName);
        }
    }
}
