using System;
using System.Diagnostics;
using System.IO;

namespace EODSettingsApp.Services
{
    /// <summary>
    /// Responsible for locating and launching the EODService executable
    /// as an independent operating-system process.
    /// </summary>
    public static class EodServiceLauncher
    {
        /// <summary>
        /// Name of the EODService executable (without path).
        /// </summary>
        private const string ExeName = "EODService.exe";

        /// <summary>
        /// Resolves the full path to EODService.exe.
        /// Resolution order:
        ///   1. Same directory as this application's executable.
        ///   2. Sibling project's Debug output (useful during development).
        /// </summary>
        /// <returns>Full path to EODService.exe.</returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the executable cannot be found at any known location.
        /// </exception>
        public static string ResolveExePath()
        {
            // 1. Next to the running WinForms app (production / published layout)
            var appDir = AppContext.BaseDirectory;
            var candidate = Path.Combine(appDir, ExeName);
            if (File.Exists(candidate))
                return candidate;

            // 2. Sibling project's Debug output (development layout inside solution)
            //    …\EODSettingsApp\bin\Debug\net10.0-windows  →  up 4 levels  →  solution root
            //    then down into EODService\bin\Debug\net10.0\EODService.exe
            var solutionRoot = Path.GetFullPath(Path.Combine(appDir, "..", "..", "..", ".."));
            var devCandidate = Path.Combine(
                solutionRoot, "EODService", "bin", "Debug", "net10.0", ExeName);

            if (File.Exists(devCandidate))
                return devCandidate;

            throw new FileNotFoundException(
                $"Could not locate '{ExeName}'.\n\n" +
                $"Searched:\n  {candidate}\n  {devCandidate}\n\n" +
                "Please build the EODService project first, or place the executable " +
                "next to EODSettingsApp.exe.",
                ExeName);
        }

        /// <summary>
        /// Launches EODService.exe as a new, independent process and returns
        /// immediately without waiting for it to complete.
        /// </summary>
        /// <param name="exePath">Full path to EODService.exe.</param>
        /// <returns>The started <see cref="Process"/> instance.</returns>
        public static Process Launch(string exePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName        = exePath,
                UseShellExecute = false,
                CreateNoWindow  = true,    // Run silently in background without pop-up console window
                WorkingDirectory = Path.GetDirectoryName(exePath)!
            };

            var process = new Process { StartInfo = startInfo };
            process.Start();
            return process;
        }
    }
}
