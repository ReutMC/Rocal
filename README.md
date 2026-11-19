# Rocal

Private, serverless Windows app for two or more computers on the same LAN: file sharing, chat and
consent-based remote desktop. No cloud, accounts or internet connection required.

> **Status: Milestone 1 (foundation) only.** See "What works today" below.

## What works today

- Solution layout, WPF shell with sidebar navigation, light-blue Rocal theme.
- English (LTR) and Persian (full RTL) localization from JSON resource files, with a language selector
  in Settings (persisted to `%AppData%\Rocal\settings.json`).
- Home page showing this computer's name, local IPv4 address and network status.
- Wire protocol foundation: length-prefixed framing and a versioned JSON envelope, with unit tests.
- CI workflow, release workflow (portable ZIP + Inno Setup installer + SHA256SUMS) and installer script.

## Not implemented yet

Device discovery, pairing/TLS, file transfer, chat, remote desktop, clipboard, diagnostics, and
most documentation (`ARCHITECTURE.md`, `SECURITY.md`, `TESTING.md`, `CONTRIBUTING.md`). Those pages in
the app say so instead of showing fake controls.

## Requirements

Windows 10/11 x64 and the .NET 10 SDK to build. Released builds are self-contained.

## Build and test

```powershell
dotnet restore Rocal.slnx
dotnet build Rocal.slnx -c Release
dotnet test Rocal.slnx -c Release
dotnet run --project src/Rocal.App
```

## Releasing

CI runs on every push to `main` and on pull requests. To publish a release:

```powershell
git tag v1.0.0
git push origin v1.0.0
```

The `Release` workflow validates the tag (`vX.Y.Z`), runs the tests, then attaches
`Rocal-v1.0.0-win-x64-portable.zip`, `Rocal-v1.0.0-win-x64-setup.exe` and `SHA256SUMS.txt` to a GitHub
Release. Tags with a suffix (for example `v1.0.0-beta1`) are marked as pre-releases.

## Layout

```
src/Rocal.App         WPF UI (MVVM, localization, theme)
src/Rocal.Core        Protocol framing and envelope
src/Rocal.Networking  Local network interface enumeration
tests/Rocal.Core.Tests
installer/            Inno Setup script
docs/NETWORK_PROTOCOL.md
```
