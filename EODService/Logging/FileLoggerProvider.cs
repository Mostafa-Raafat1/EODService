using System;
using System.IO;
using Microsoft.Extensions.Logging;
using EODService.Config;

namespace EODService.Logging
{
    /// <summary>
    /// File logger provider that writes service logs into a structured folder hierarchy:
    ///   {LogFolderPath}\{yyyy-MM}\{yyyy-MM-dd}.txt
    ///
    /// Log format per line:
    ///   HH:mm:ss │ LEVEL   │ ShortClassName        │ message
    ///
    /// A run banner is written once at startup via WriteRunBanner() to clearly
    /// separate consecutive runs in the same daily file.
    /// A new daily file is created automatically at midnight without a restart.
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        private static readonly object _lock = new object();

        // Column widths for aligned table layout
        private const int LevelWidth    = 7;  // "INFO   ", "WARNING", "ERROR  "
        private const int CategoryWidth = 25; // padded class name

        /// <summary>
        /// Resolves the full path for today's log file.
        /// Example: C:\EODConfig\Logs\2026-08\2026-08-13.txt
        /// </summary>
        public static string GetTodayLogFilePath()
        {
            var today      = DateTime.Now;
            var monthFolder = today.ToString("yyyy-MM");           // e.g. 2026-08
            var dayFile     = today.ToString("yyyy-MM-dd") + ".txt"; // e.g. 2026-08-13.txt
            return Path.Combine(PathsConfig.LogFolderPath, monthFolder, dayFile);
        }

        /// <summary>
        /// Writes a prominent run-start banner to today's log file.
        /// </summary>
        public static void WriteRunBanner()
        {
            var now    = DateTime.Now;
            var line   = "================================================================================";
            var header = $"  🚀 EOD SERVICE EXECUTION RUN  │  {now:yyyy-MM-dd  HH:mm:ss}";

            var banner = Environment.NewLine
                       + line                   + Environment.NewLine
                       + header                 + Environment.NewLine
                       + line                   + Environment.NewLine;

            AppendToFile(banner);
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        /// <summary>Strips namespace prefix, returning only the class name.</summary>
        private static string ShortName(string categoryName)
        {
            var dot = categoryName.LastIndexOf('.');
            return dot >= 0 ? categoryName[(dot + 1)..] : categoryName;
        }

        /// <summary>Converts LogLevel to a short, fixed-width bracketed label.</summary>
        private static string LevelLabel(LogLevel level) => level switch
        {
            LogLevel.Trace       => "[TRACE]",
            LogLevel.Debug       => "[DEBUG]",
            LogLevel.Information => "[INFO ]",
            LogLevel.Warning     => "[WARN ]",
            LogLevel.Error       => "[ERROR]",
            LogLevel.Critical    => "[FATAL]",
            _                    => "[OTHER]"
        };

        private static void AppendToFile(string content)
        {
            lock (_lock)
            {
                try
                {
                    var filePath = GetTodayLogFilePath();
                    var dir      = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using var writer = new StreamWriter(fs);
                    writer.Write(content);
                }
                catch (Exception ex)
                {
                    // Best-effort trace fallback — do not crash the service on a logging failure.
                    System.Diagnostics.Trace.WriteLine($"[FileLoggerProvider] Logging to file failed: {ex.Message}");
                }
            }
        }

        // ── ILoggerProvider ──────────────────────────────────────────────────────

        public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName);

        public void Dispose() { }

        // ── Inner logger ─────────────────────────────────────────────────────────

        private class FileLogger : ILogger
        {
            private readonly string _categoryName;

            public FileLogger(string categoryName) => _categoryName = categoryName;

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;

                var message   = formatter(state, exception);
                var time      = DateTime.Now.ToString("HH:mm:ss");
                var level     = LevelLabel(logLevel);
                var category  = ShortName(_categoryName).PadRight(CategoryWidth);

                var logLine   = $"{time} │ {level} │ {category} │ {message}";

                if (exception != null)
                    logLine += Environment.NewLine + exception.ToString();

                AppendToFile(logLine + Environment.NewLine);
            }
        }
    }
}
