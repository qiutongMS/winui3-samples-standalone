# CameraAdvancedCapture — WinUI 3 (migrated from UWP)

**Quality: 95 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 10 / 10 | 8 / 10 | 8 / 10 |

Capture photos with advanced scene modes (HDR, low-light, exposure bracketing) via `MediaCapture.AdvancedPhotoCapture`.

## Run

```powershell
winapp run
```

## Before / after (main view)

UWP source (baseline) and the WinUI 3 migration of the same scenario:

| UWP source | WinUI 3 (this migration) |
|:-:|:-:|
| ![UWP main view](docs/before-main.png) | ![WinUI 3 main view](docs/after-main.png) |

<!-- BEGIN per-scenario -->
## Per-scenario UWP -> WinUI 3 comparison

Initial state of each scenario page, original UWP sample on the left and the migrated WinUI 3 build on the right.

### Scenario 1 — MainPage

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| _(no UWP baseline captured)_ | ![WinUI 3 — Scenario 1 — MainPage](docs/after-scenarios/01_MainPage.png) |
<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\CameraAdvancedCapture\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run33)
- Project file: `CameraAdvancedCapture\CameraAdvancedCapture.csproj`


