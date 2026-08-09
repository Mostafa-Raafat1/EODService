using System;
using System.IO;

namespace EODSettingsApp.AppSettingsConfig
{
    /// <summary>
    /// Single source of truth for the path to EODService's AppSettings.json.
    ///
    /// Resolution order:
    ///   1. Same folder as the running EODSettingsApp.exe (production / published layout).
    ///   2. EODService project source folder inside the solution (development layout).
    /// </summary>
    public static class AppSettingsPath
    {
        private const string FileName          = "AppSettings.json";
        private const string EodServiceProject = "EODService";

        /// <summary>
        /// Returns the full path to AppSettings.json.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the file cannot be found at any known location.
        /// </exception>
        public static string Resolve()
        {
            // 1. Production: AppSettings.json published next to EODSettingsApp.exe.
            var appDir           = AppContext.BaseDirectory;
            var productionPath   = Path.Combine(appDir, FileName);
            if (File.Exists(productionPath))
                return productionPath;

            // 2. Development: navigate from
            //    …\EODSettingsApp\bin\Debug\net10.0-windows  (4 levels up)
            //    to the solution root, then into the EODService project source folder.
            var solutionRoot     = Path.GetFullPath(Path.Combine(appDir, "..", "..", "..", ".."));
            var developmentPath  = Path.Combine(solutionRoot, EodServiceProject, FileName);
            if (File.Exists(developmentPath))
                return developmentPath;

            throw new FileNotFoundException(
                $"Could not locate '{FileName}'.\n\n" +
                $"Searched:\n" +
                $"  {productionPath}\n" +
                $"  {developmentPath}",
                FileName);
        }
    }
}
