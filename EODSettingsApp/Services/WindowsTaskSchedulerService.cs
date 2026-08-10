using System;
using System.Collections.Generic;
using System.IO;
using EODSettingsApp.AppSettingsConfig;
using Microsoft.Win32.TaskScheduler;

namespace EODSettingsApp.Services
{
    /// <summary>
    /// Integrates with native Windows Task Scheduler to register or remove
    /// automated background execution triggers for EODService.exe.
    /// </summary>
    public static class WindowsTaskSchedulerService
    {
        private const string TaskName = "EODService_AutoImport";

        /// <summary>
        /// Registers or updates the Windows Task Scheduler task using the configured ScheduleSettings.
        /// Uses standard user privileges (TaskRunLevel.LUA) so administrator elevation is not required.
        /// </summary>
        public static void RegisterOrUpdateTask(ScheduleSettingsSection settings, string exePath)
        {
            try
            {
                using var ts = new TaskService();

                // If disabled by user, remove existing task if present
                if (!settings.Enabled || settings.WorkingDays == null || settings.WorkingDays.Count == 0)
                {
                    ts.RootFolder.DeleteTask(TaskName, false);
                    return;
                }

                // Create new Task Definition
                var td = ts.NewTask();
                td.RegistrationInfo.Description = "Automated EOD Stock Market Data Import Task";
                
                // Use standard user level (LUA) so no admin elevation is required
                td.Principal.RunLevel = TaskRunLevel.LUA;

                // Parse run time (default: 18:00:00)
                if (!TimeSpan.TryParse(settings.RunTime, out var runTime))
                {
                    runTime = new TimeSpan(18, 0, 0);
                }

                // Create Weekly Trigger with specified working days
                var trigger = new WeeklyTrigger
                {
                    StartBoundary = DateTime.Today.Add(runTime),
                    DaysOfWeek = ConvertToDaysOfWeekFlags(settings.WorkingDays),
                    WeeksInterval = 1
                };
                td.Triggers.Add(trigger);

                // Configure Action: Run EODService.exe
                var workingDir = Path.GetDirectoryName(exePath);
                td.Actions.Add(new ExecAction(exePath, arguments: null, workingDirectory: workingDir));

                // Register (or overwrite) task in Windows Task Scheduler
                ts.RootFolder.RegisterTaskDefinition(TaskName, td);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException(
                    "Access Denied: Please run EODSettingsApp as Administrator, or check folder permissions.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to update Windows Task Scheduler:\n{ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts day name strings into TaskScheduler's DaysOfTheWeek bitwise enum flags.
        /// </summary>
        private static DaysOfTheWeek ConvertToDaysOfWeekFlags(List<string> workingDays)
        {
            DaysOfTheWeek flags = 0;
            foreach (var dayName in workingDays)
            {
                if (Enum.TryParse<DayOfWeek>(dayName, true, out var dayOfWeek))
                {
                    flags |= dayOfWeek switch
                    {
                        DayOfWeek.Monday    => DaysOfTheWeek.Monday,
                        DayOfWeek.Tuesday   => DaysOfTheWeek.Tuesday,
                        DayOfWeek.Wednesday => DaysOfTheWeek.Wednesday,
                        DayOfWeek.Thursday  => DaysOfTheWeek.Thursday,
                        DayOfWeek.Friday    => DaysOfTheWeek.Friday,
                        DayOfWeek.Saturday  => DaysOfTheWeek.Saturday,
                        DayOfWeek.Sunday    => DaysOfTheWeek.Sunday,
                        _ => 0
                    };
                }
            }
            return flags;
        }
    }
}
