# EOD Service — Installation Guide

## ⚠️ Important: Use Setup.exe

To install the **EOD Service**, always use:

**`Setup.exe`**

Do **not** install the service by opening `EODInstaller.msi` directly.

### Why?

`Setup.exe` is the recommended installer because it handles the installation process and its required prerequisites more safely and reliably.

The `.msi` file is an installation package used by the setup process. It is **not intended to be launched directly by the end user**.

## Installation Steps

1. Extract the downloaded EOD Service installation package.
2. Locate **`Setup.exe`**.
3. Right-click `Setup.exe` and select **Run as administrator** if Windows requests elevated permissions.
4. Follow the installation wizard.
5. Wait for the installation to complete.
6. The EOD Service and its required components will then be installed.

### Do Not

Do not open:

`EODInstaller.msi`

directly to install the service.

### Do

Open:

`Setup.exe`

and follow the installation wizard.

## Distribution

When distributing the EOD Service to another user, provide the **complete installation package**, including `Setup.exe` and the associated installer files.

Do not send only the `.msi` file.

> **Recommended:** Keep `Setup.exe` and `EODInstaller.msi` in the same folder when running the installation.
