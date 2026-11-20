# Build failure analysis (Phase 0)

## Original failure

The first CI run on `main` (commit `e39d9ca`, "Milestone 1") failed after the initial push. The workflow
that failed was `CI` (`.github/workflows/ci.yml`), job `build`, at the **Build (Release)** step. A later
**Publish win-x64** step would have failed for the same reason.

How this was diagnosed: the GitHub connection available to the assistant could not download Actions
logs directly, so a temporary workflow (`diagnose.yml`) was added on a branch and opened as pull request #1.
It re-ran restore, build, test and publish, captured each step's output, and posted the errors back as a PR
comment. The `CI` build job on that PR failed the same way as the original run, with the same unmodified
source code, so the failure reproduced. The original run's own log was not read, so "same cause" rests on
identical source and workflow rather than on that specific log.

Environment of the failing run: GitHub-hosted `windows-latest`, .NET SDK 10.0.404, MSBuild 18.9.

| Step    | Result before the fix |
|---------|-----------------------|
| restore | succeeded             |
| build   | **failed**, 9 errors  |
| test    | succeeded, 20/20 tests passed (the test project does not reference the WPF app) |
| publish | **failed**, same errors |

Errors (all in `Rocal.App`, reported in the WPF markup-compile temp project `Rocal.App_*_wpftmp.csproj`):

```
SettingsService.cs: error CS0103: The name 'Path' / 'File' / 'Directory' does not exist in the current context
SettingsService.cs: error CS0103: The name 'IOException' does not exist in the current context
LocalizationService.cs: error CS0246: The type or namespace name 'FileNotFoundException' could not be found
```

## Root cause

`Rocal.App` enables `<ImplicitUsings>` through `Directory.Build.props`, and the code relied on
`System.IO` being one of the implicit usings (as it is in console, library and test projects).

For WPF projects (`<UseWPF>true</UseWPF>`) the SDK deliberately leaves `System.IO` (and `System.Net.Http`) out
of the implicit usings, because their type names collide with WPF types such as `System.Windows.Shapes.Path`.
So `Path`, `File`, `Directory`, `IOException` and `FileNotFoundException` were unresolved in the two files
that used them. The code had never been compiled before the first CI run (no .NET SDK was available where it
was written), so nothing caught this earlier.

## Affected components

- `src/Rocal.App/Services/SettingsService.cs`
- `src/Rocal.App/Services/LocalizationService.cs`

Not affected: `Rocal.Core`, `Rocal.Networking` and `Rocal.Core.Tests` (they use the standard implicit usings).

## Fix

Added an explicit `using System.IO;` to both files. No tests were disabled or changed, no warnings were
suppressed, and no functionality was replaced.

## Verification

Run on GitHub-hosted `windows-latest` (SDK 10.0.404) against the fix commit, via PR #1:

| Step    | Result |
|---------|--------|
| restore | exit 0 |
| build (Release) | exit 0, 0 warnings, 0 errors |
| test    | exit 0, 20 passed, 0 failed, 0 skipped |
| publish (win-x64, self-contained) | exit 0 |
| `CI` workflow `build` job | success |

## Remaining problems and limits

- The app has been compiled and published by CI but **not launched or visually checked** on a Windows
  machine. Sidebar icon glyphs, RTL layout and the theme are unverified at runtime.
- The release workflow (`release.yml`, Inno Setup installer, checksums, GitHub Release) has **not yet run**.
- No test project covers `Rocal.App` or `Rocal.Networking`.
- The NuGet package versions pinned in the project files restored successfully, so those were not the problem.
