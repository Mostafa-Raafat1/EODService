using System;
using Microsoft.Extensions.Logging;

namespace EODSettingsApp.Logging
{
    public class UiLoggerProvider : ILoggerProvider
    {
        public Action<string>? OnLog { get; set; }

        public ILogger CreateLogger(string categoryName)
        {
            return new UiLogger(this);
        }

        public void Dispose() { }
    }

    public class UiLogger : ILogger
    {
        private readonly UiLoggerProvider _provider;

        public UiLogger(UiLoggerProvider provider)
        {
            _provider = provider;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            
            // Format to look like console output
            string prefix = logLevel switch
            {
                LogLevel.Information => "info: ",
                LogLevel.Warning => "warn: ",
                LogLevel.Error => "fail: ",
                _ => ""
            };

            var fullMessage = $"{prefix}{message}";
            
            _provider.OnLog?.Invoke(fullMessage);
        }
    }
}
