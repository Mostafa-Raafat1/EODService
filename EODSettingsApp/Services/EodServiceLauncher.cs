using System;
using System.Diagnostics;
using System.IO;
using EODService.Config;

namespace EODSettingsApp.Services
{
    /// <summary>
    /// Responsible for locating and launching the EODService executable
    /// as an independent operating-system process.
    /// </summary>
    public static class EodServiceLauncher
    {
        /// <summary>
        /// Default name of the EODService executable (without path).
        /// </summary>
        private const string DefaultExeName = "EODService.exe";

        /// <summary>
        /// Resolves the full path to EODService.exe.
        /// Resolution order:
        ///   1. Path configured in PathesConfig.json (via PathsConfig.EODServicePath).
        ///   2. Same directory as this application's executable.
        ///   3. Sibling project's Debug / Release output (useful during development).
        /// </summary>
        /// <returns>Full path to EODService.exe.</returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the executable cannot be found at any known location.
        /// </exception>
        public static string ResolveExePath()
        {
            var rawConfigPath = PathsConfig.Current?.EODServicePath;

            // 1. If EODServicePath in PathesConfig.json is explicitly blank or whitespace, fail fast
            if (string.IsNullOrWhiteSpace(rawConfigPath))
            {
                throw new InvalidOperationException(
                    "EODServicePath is blank or missing in PathesConfig.json. " +
                    "Please configure the path to EODService.exe in PathesConfig.json (e.g., \"EODServicePath\": \"EODService.exe\").");
            }

            // 2. Check direct candidate in application running directory (highest priority for relative config paths)
            var appDir = AppContext.BaseDirectory;
            var localCandidate = Path.GetFullPath(Path.Combine(appDir, rawConfigPath));
            if (File.Exists(localCandidate))
            {
                return localCandidate;
            }

            // 3. Resolve configured path via PathsConfig
            var configuredPath = PathsConfig.EODServicePath;
            if (!string.IsNullOrWhiteSpace(configuredPath) && File.Exists(configuredPath))
            {
                return configuredPath;
            }

            // 4. Development fallback: Dynamically search parent folders for the solution root to locate sibling project output
            var solutionRoot = FindSolutionRoot(appDir);

            var candidates = new[]
            {
                Path.Combine(solutionRoot, "EODService", "bin", "Debug", "net10.0", "win-x64", DefaultExeName),
                Path.Combine(solutionRoot, "EODService", "bin", "Debug", "net10.0", DefaultExeName),
                Path.Combine(solutionRoot, "EODService", "bin", "Release", "net10.0", "win-x64", DefaultExeName),
                Path.Combine(solutionRoot, "EODService", "bin", "Release", "net10.0", DefaultExeName)
            };

            foreach (var devCandidate in candidates)
            {
                if (File.Exists(devCandidate))
                    return devCandidate;
            }

            throw new FileNotFoundException(
                $"Could not locate EODService executable specified in PathesConfig.json ('{rawConfigPath}').\n\n" +
                $"Resolved location: {configuredPath}\n" +
                $"Dev search locations:\n  {string.Join("\n  ", candidates)}\n\n" +
                "Please verify the EODServicePath setting in PathesConfig.json or build EODService.csproj.",
                configuredPath);
        }

        private static string FindSolutionRoot(string startDir)
        {
            var current = new DirectoryInfo(startDir);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "EODService.sln")) ||
                    File.Exists(Path.Combine(current.FullName, "EODService.slnx")))
                {
                    return current.FullName;
                }

                var eodSubFolder = Path.Combine(current.FullName, "EODService");
                if (Directory.Exists(eodSubFolder) && File.Exists(Path.Combine(eodSubFolder, "EODService.csproj")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
            return startDir;
        }

        /// <summary>
        /// Checks if an instance of EODService.exe is currently running.
        /// </summary>
        /// <returns>True if at least one EODService process is active, false otherwise.</returns>
        public static bool IsRunning()
        {
            var processName = Path.GetFileNameWithoutExtension(DefaultExeName);
            var processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
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
