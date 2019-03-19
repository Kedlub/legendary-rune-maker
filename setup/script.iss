; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Legendary Rune Maker"
#define MyAppVersion "1.3.0"
#define MyAppPublisher "pipe01"
#define MyAppURL "https://github.com/pipe01"
#define MyAppExeName "Legendary Rune Maker.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{083C7EE0-3D25-4DF2-AFA2-13A59D50CEB8}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userappdata}\{#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=lrm-setup-{#MyAppVersion}
Compression=lzma
SolidCompression=yes
PrivilegesRequired=lowest

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[CustomMessages]
StartAtStartup=Undefined
english.StartAtStartup=Start {#MyAppName} at Windows startup
spanish.StartAtStartup=Iniciar {#MyAppName} al inicio de Windows

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1
Name: "StartMenuEntry" ; Description: "{cm:StartAtStartup}" ; GroupDescription: "{cm:AdditionalIcons}"; MinVersion: 4,4; Flags: unchecked

[Files]
Source: "..\Legendary Rune Maker\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[UninstallDelete]
Type: filesandordirs; Name: "{app}\*"

[Icons]
Name: "{commonprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon
Name: "{userstartup}\Legendary Rune Maker"; Filename: "{app}\{#MyAppExeName}"; Tasks:StartMenuEntry;

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

