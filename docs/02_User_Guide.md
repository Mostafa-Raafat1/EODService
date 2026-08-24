# User Guide: EOD Service Manager

This guide explains how to use the **EOD Service Manager** (WinForms GUI) to manage your End-of-Day data imports.

## 1. Initial Setup and Database Connection
Before the service can run, it must be connected to the Oracle Database.
1. Open the application and navigate to the **Database Settings** tab.
2. Enter the Host, Port, Service Name, Username, and Password.
3. Click **Test Connection**. 
4. Once successful, click **Save Connection**.

## 2. Managing Data Providers
The application supports multiple data providers (e.g., Yahoo Finance, Twelve Data, Reuters).
1. Navigate to the **Provider Settings** tab.
2. You will see tabs for each supported provider. Here you can configure:
   - **Base URL & Endpoint**: The API address.
   - **API Key**: Required for premium APIs like Twelve Data.
   - **Parameters (JSON)**: Specific API parameters (e.g., intervals, output size).
3. Ensure the JSON format is valid before clicking **Save Provider Settings**.

## 3. Managing Stock Symbols
The **Symbol Settings** tab allows you to control which stocks are processed.
1. The grid displays all stocks currently stored in the `EOD_STOCKS` database table.
2. You can toggle the active flags (e.g., `YF_FLAG`, `TD_FLAG`, `REUTER_FLAG`) to enable/disable data fetching for specific providers.
3. **Add/Delete**: Use the grid to add new symbols or remove obsolete ones.
4. Click **Save Changes** to commit updates to the database.

## 4. Scheduling Automated Runs
1. On the **Main Dashboard**, locate the Schedule settings.
2. Check the box to **Enable Schedule**.
3. Select the working days (e.g., Monday through Friday).
4. Set the precise **Run Time**.
5. Click **Save Schedule**. This registers a background task in Windows Task Scheduler.

## 5. Manual Execution and Monitoring
- **Run Now**: You can trigger a manual import at any time by clicking the "Run Now" button on the main dashboard.
- **Live Logs**: The bottom panel on the main dashboard displays real-time logs. It automatically polls the log file every 2 seconds while a run is executing.
