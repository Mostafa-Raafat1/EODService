[Setup]
; Basic application details
AppName=EOD Service Manager
AppVersion=1.0
AppPublisher=Youssef Azzab
; Install to user's AppData - NO admin/UAC elevation required (works seamlessly on any PC)
DefaultDirName={localappdata}\EODServiceManager
DefaultGroupName=EOD Service Manager
PrivilegesRequired=lowest
ArchitecturesInstallIn64BitMode=x64
; Output settings for the generated setup.exe
OutputDir=Output
OutputBaseFilename=EODServiceManager_Setup
SetupIconFile=EODSettingsApp\TICKR.ico
UninstallDisplayIcon={app}\EODServiceManager.exe
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
RestartApplications=no

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; Package the self-contained Windows Forms app (EODServiceManager) + EODService engine + dependencies
Source: "publish_forms\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; Start Menu Shortcut
Name: "{group}\EOD Service Manager"; Filename: "{app}\EODServiceManager.exe"; IconFilename: "{app}\EODServiceManager.exe"
; Desktop Shortcut
Name: "{autodesktop}\EOD Service Manager"; Filename: "{app}\EODServiceManager.exe"; IconFilename: "{app}\EODServiceManager.exe"; Tasks: desktopicon

[Run]
; Launch the application after installation finishes
Filename: "{app}\EODServiceManager.exe"; Description: "{cm:LaunchProgram,EOD Service Manager}"; Flags: nowait postinstall skipifsilent
