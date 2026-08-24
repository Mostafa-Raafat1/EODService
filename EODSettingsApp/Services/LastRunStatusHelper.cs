using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EODService.Config;

namespace EODSettingsApp.Services
{
    public class LastRunInfo
    {
        public bool HasRun { get; set; }
        public DateTime? Timestamp { get; set; }
        public bool IsSuccess { get; set; }
        public int RecordCount { get; set; }
        public string SummaryText { get; set; } = "⚪ Last Run: No logs recorded yet";
        public string? LogFilePath { get; set; }
    }

    /// <summary>
    /// Scans the log directory to determine the status, timestamp, and results of the most recent service execution.
    /// </summary>
    public static class LastRunStatusHelper
    {
        private static readonly Regex RunBannerRegex = new(@"RUN STARTED\s*│\s*(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})", RegexOptions.Compiled);
        private static readonly Regex RecordCountRegex = new(@"Total records collected:\s*(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SavedRecordsRegex = new(@"transaction for\s*(\d+)\s*record", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static LastRunInfo GetLastRunInfo()
        {
            var info = new LastRunInfo();

            try
            {
                var logFolder = PathsConfig.LogFolderPath;
                if (!Directory.Exists(logFolder))
                {
                    return info;
                }

                // Find all log files sorted by newest first
                var allLogs = Directory.GetFiles(logFolder, "*.txt", SearchOption.AllDirectories)
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.LastWriteTime)
                    .ToList();

                if (!allLogs.Any())
                {
                    return info;
                }

                var latestLogFile = allLogs.First();
                info.LogFilePath = latestLogFile.FullName;

                string[] lines;
                using (var fs = new FileStream(latestLogFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs))
                {
                    var content = reader.ReadToEnd();
                    lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                }

                if (lines.Length == 0)
                {
                    return info;
                }

                info.HasRun = true;

                // Find the latest RUN STARTED banner
                int lastRunStartIndex = 0;
                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    var match = RunBannerRegex.Match(lines[i]);
                    if (match.Success && DateTime.TryParse(match.Groups[1].Value, out var dt))
                    {
                        info.Timestamp = dt;
                        lastRunStartIndex = i;
                        break;
                    }
                }

                info.Timestamp ??= latestLogFile.LastWriteTime;

                // Scan only lines for the latest run
                bool hasCompleted = false;
                bool hasError = false;
                bool hasWarning = false;

                for (int i = lastRunStartIndex; i < lines.Length; i++)
                {
                    var line = lines[i];

                    if (line.Contains("Database save completed successfully", StringComparison.OrdinalIgnoreCase) ||
                        line.Contains("EOD import complete", StringComparison.OrdinalIgnoreCase))
                    {
                        hasCompleted = true;
                    }

                    if (line.Contains("ERROR  ", StringComparison.OrdinalIgnoreCase) ||
                        line.Contains("FATAL  ", StringComparison.OrdinalIgnoreCase) ||
                        line.Contains("Exception", StringComparison.OrdinalIgnoreCase))
                    {
                        hasError = true;
                    }

                    if (line.Contains("WARN  ", StringComparison.OrdinalIgnoreCase) ||
                        line.Contains("WARNING", StringComparison.OrdinalIgnoreCase))
                    {
                        hasWarning = true;
                    }

                    var countMatch = RecordCountRegex.Match(line);
                    if (countMatch.Success && int.TryParse(countMatch.Groups[1].Value, out int count))
                    {
                        info.RecordCount = count;
                    }

                    var saveMatch = SavedRecordsRegex.Match(line);
                    if (saveMatch.Success && int.TryParse(saveMatch.Groups[1].Value, out int saveCount))
                    {
                        info.RecordCount = saveCount;
                    }
                }

                info.IsSuccess = hasCompleted && !hasError && !hasWarning;

                var timeStr = info.Timestamp.Value.Date == DateTime.Today
                    ? $"Today at {info.Timestamp.Value:HH:mm:ss}"
                    : $"{info.Timestamp.Value:yyyy-MM-dd HH:mm}";

                if (info.IsSuccess)
                {
                    info.SummaryText = $"🟢 Last Run: {timeStr} — ✔ Success ({info.RecordCount} record(s) synced)";
                }
                else if (hasCompleted && (hasError || hasWarning))
                {
                    info.SummaryText = $"🟡 Last Run: {timeStr} — Completed with Warnings ({info.RecordCount} records)";
                }
                else if (hasError)
                {
                    info.SummaryText = $"🔴 Last Run: {timeStr} — Failed (See logs for details)";
                }
                else
                {
                    info.SummaryText = $"🔵 Last Run: {timeStr} — In Progress or Idle";
                }
            }
            catch (Exception ex)
            {
                info.SummaryText = $"⚠️ Could not parse last run: {ex.Message}";
            }

            return info;
        }
    }
}
