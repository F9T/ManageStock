; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "ManageStock"
#define MyAppVersion "1.0"
#define MyAppPublisher "Thomas Lovis"
#define MyAppExeName "ManageStock.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{350B30F3-9C2C-45F2-A154-319924611591}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DisableDirPage=yes
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=Output
OutputBaseFilename=ManageStock
SetupIconFile=..\Application.GUI\AppIcon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
   
[Dirs]
Name: "{userappdata}\ManageStock"

[Files]                                                   
Source: "..\Application.GUI\bin\Release\Application.GUI.exe"; DestDir: "{app}"; DestName: "{#MyAppExeName}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\Antlr3.Runtime.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\Application.Backup.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\Application.CommandManager.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\Application.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\Application.Excel.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\EntityFramework.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\EntityFramework.SqlServer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\FastColoredTextBox.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\ManageStock.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\MaterialDesignColors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\MaterialDesignExtensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\MaterialDesignThemes.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\Microsoft.Xaml.Behaviors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\Notification.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\OrderTracking.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\System.Data.SQLite.EF6.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\System.Data.SQLite.Linq.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\System.ValueTuple.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\unvell.ReoGrid.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\unvell.ReoGridEditor.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\unvell.ReoScript.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\unvell.ReoScript.EditorLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "settings.xml"; DestDir: "{userappdata}\ManageStock"; Flags: ignoreversion
Source: "share.cmd"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Application.GUI\bin\Release\x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Application.GUI\bin\Release\x86\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]         
Filename: "{app}\share.cmd"; Description: "Ajouter le disque Y:"; Flags: nowait postinstall runhidden
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

