# Administrator & Troubleshooting Guide

This guide covers deployment, file paths, and troubleshooting for System Administrators.

## Deployment & Installation
The application is packaged using Inno Setup (`EODServiceInstaller.iss`). 
- **Installation Path**: `%LocalAppData%\EODServiceManager`
- **Privileges**: Runs and installs at the user level (`PrivilegesRequired=lowest`). Administrator elevation is **not** required.

## Crucial File Paths
The system relies on specific configuration files:
- `C:\EODConfig\settings.json`: Stores the `ActiveProvider` ID. If this is missing or inaccessible, the console app will fail to start.
- `C:\EODConfig\Logs\`: The default directory for rolling daily log files. Format: `{yyyy-MM}\{yyyy-MM-dd}.txt`.
- `AppSettings.json`: Located in the application binary directory. Contains the connection string and schedule details.

## Windows Task Scheduler
The WinForms application manages a background task to automate the EOD runs.
- **Task Name**: `EODService_AutoImport`
- **Trigger**: WeeklyTrigger (based on configured working days).
- **Run Level**: Standard User (LUA).

*Troubleshooting*: If the automated run does not trigger, open Windows Task Scheduler (`taskschd.msc`), locate `EODService_AutoImport`, and verify the "Next Run Time" and "Last Run Result".

## FAQs & Common Errors

**Q: The application says "No providers were found in the database."**
A: Ensure the database connection is valid and that the `PROVIDER` table has been populated with the default provider rows (Yahoo, TwelveData, Reuters).

**Q: JSON Validation Error when saving provider settings.**
A: The parameters field requires strict, well-formed JSON. Ensure keys and string values are wrapped in double quotes (`"key": "value"`).

**Q: The Console App flashes and closes immediately without fetching data.**
A: Check the log file in `C:\EODConfig\Logs`. The most common causes are an invalid database connection string, or `C:\EODConfig\settings.json` is missing or unreadable.
