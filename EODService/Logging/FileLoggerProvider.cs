using System;
using System.IO;
using Microsoft.Extensions.Logging;
using EODService.Config;

namespace EODService.Logging
{
    /// <summary>
    /// File logger provider that writes all service logs to eod_service.log in the config directory.
    /// This allows EODSettingsApp to read and present background scheduled run logs in real-time.
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        private static readonly object _lock = new object();

        public static string LogFilePath =>
            Path.Combine(PathesConfig.ActiveProviderFolderPath, "eod_service.log");

        public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName);

        public void Dispose() { }

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

                var message = formatter(state, exception);
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var logLine = $"[{timestamp}] [{logLevel}] {message}";

                if (exception != null)
                {
                    logLine += Environment.NewLine + exception.ToString();
                }

                lock (_lock)
                {
                    try
                    {
                        var dir = Path.GetDirectoryName(LogFilePath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        File.AppendAllText(LogFilePath, logLine + Environment.NewLine);
                    }
                    catch
                    {
                        // Best effort log file write
                    }
                }
            }
        }
    }
}
