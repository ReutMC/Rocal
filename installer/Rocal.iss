; Built by .github/workflows/release.yml. Defines are passed on the ISCC command line.
#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif
#ifndef SourceDir
  #define SourceDir "..\artifacts\publish"
#endif
#ifndef OutputDir
  #define OutputDir "..\release"
#endif
#ifndef OutputName
  #define OutputName "Rocal-setup"
#endif

[Setup]
AppId={{7C2E4B1A-5D3F-4A8E-9B6C-2F1D0E8A4C37}
AppName=Rocal
AppVersion={#AppVersion}
AppPublisher=Rocal
DefaultDirName={autopf}\Rocal
DefaultGroupName=Rocal
UninstallDisplayIcon={app}\Rocal.exe
OutputDir={#OutputDir}
OutputBaseFilename={#OutputName}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; Flags: unchecked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Rocal"; Filename: "{app}\Rocal.exe"
Name: "{autodesktop}\Rocal"; Filename: "{app}\Rocal.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\Rocal.exe"; Description: "Launch Rocal"; Flags: nowait postinstall skipifsilent
