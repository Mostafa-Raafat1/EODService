[Setup]
; Basic application details
AppName=EOD Service Manager
AppVersion=1.0
AppPublisher=Youssef Azzab
; Install to user's AppData - NO admin/UAC prompt required
DefaultDirName={localappdata}\EODServiceManager
; Start menu group name
DefaultGroupName=EOD Service Manager
; Never ask for admin/UAC elevation
PrivilegesRequired=lowest
; Output settings for the generated setup.exe
OutputDir=Output
OutputBaseFilename=EODServiceManager_Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Package the Windows Forms app (EODServiceManager)
Source: "publish_forms\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; Start Menu Icon
Name: "{group}\EOD Service Manager"; Filename: "{app}\EODServiceManager.exe"
; Desktop Icon
Name: "{autodesktop}\EOD Service Manager"; Filename: "{app}\EODServiceManager.exe"; Tasks: desktopicon

[Run]
; Launch the app after installation finishes
Filename: "{app}\EODServiceManager.exe"; Description: "{cm:LaunchProgram,EOD Service Manager}"; Flags: nowait postinstall skipifsilent
