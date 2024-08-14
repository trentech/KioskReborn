; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "KioskReborn"
#define MyAppPublisher "terrence.monroe@ljungstrom.com"
#define MyAppExeName "KioskReborn.exe"
#define MyAppVersion GetVersionNumbersString("..\KioskReborn\bin\Release\KioskReborn.exe")

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{EBFE5E66-17FB-47DB-951F-2EC240CCC411}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
ArchitecturesInstallIn64BitMode=x64
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DisableDirPage=yes
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=.\
OutputBaseFilename=KioskReborn_Setup_{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\KioskReborn\bin\Release\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\KioskReborn\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\KioskReborn\bin\Release\Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion
Source: "..\KioskReborn\bin\Release\runtimes\*"; DestDir: "{app}\runtimes"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\KioskRebornTask\bin\Release\*"; DestDir: "{app}\Update"; Flags: ignoreversion recursesubdirs createallsubdirs; Check: CmdLineParamExists('/Service')

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall runasoriginaluser; Check: CmdLineParamExists('/Launch')
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -Command $Trigger = New-ScheduledTaskTrigger -Daily -At ""00:00:00""; $Action = New-ScheduledTaskAction -Execute 'C:\Program Files\KioskReborn\Update\KioskRebornTask.exe'; Register-ScheduledTask -TaskName ""KioskRebornUpdate"" -Trigger $Trigger -Action $Action -User ""SYSTEM"" -RunLevel Highest;"; \
WorkingDir: {app}; Flags: runhidden; Check: CmdLineParamExists('/Service')

;Filename: "sc"; Parameters: "create KioskRebornUpdater start= auto DisplayName= KioskRebornUpdater binPath= ""C:\Program Files\KioskReborn\Update\KioskRebornUpdater.exe"""; Flags: runhidden postinstall; Check: CmdLineParamExists('/Service')
;Filename: "sc"; Parameters: "config KioskRebornUpdater displayname= KioskRebornUpdater"; Flags: runhidden; Check: CmdLineParamExists('/Service')
;Filename: "sc"; Parameters: "start KioskRebornUpdater"; Flags: runhidden; Check: CmdLineParamExists('/Service')

[UninstallRun]
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -Command Unregister-ScheduledTask -TaskName ""KioskRebornUpdate"" -Confirm:$false"; WorkingDir: {app}; Flags: runhidden;
;Filename: "sc"; Parameters: "stop KioskRebornUpdater"; Flags: runhidden;
;Filename: "sc"; Parameters: "delete KioskRebornUpdater"; Flags: runhidden;

[Code]
function CmdLineParamExists(const Value: string): Boolean;
var
  I: Integer;  
begin
  Result := False;
  for I := 1 to ParamCount do
    if CompareText(ParamStr(I), Value) = 0 then
    begin
      Result := True;
      Exit;
    end;
end;