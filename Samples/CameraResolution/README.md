# CameraResolution — WinUI 3 (migrated from UWP)

**Quality: 93 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 9 / 10 | 8 / 10 | 7 / 10 |

Change the resolution of a capture device (preview, photo, video streams).

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

### Scenario 1 — Change camera preview settings

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 1 — Change camera preview settings](docs/before-scenarios/01_Change_camera_preview_settings.png) | ![WinUI 3 — Scenario 1 — Change camera preview settings](docs/after-scenarios/01_Change_camera_preview_settings.png) |

### Scenario 2 — Change preview and photo settings

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 2 — Change preview and photo settings](docs/before-scenarios/02_Change_preview_and_photo_settings.png) | ![WinUI 3 — Scenario 2 — Change preview and photo settings](docs/after-scenarios/02_Change_preview_and_photo_settings.png) |

### Scenario 3 — Match aspect ratios

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 3 — Match aspect ratios](docs/before-scenarios/03_Match_aspect_ratios.png) | ![WinUI 3 — Scenario 3 — Match aspect ratios](docs/after-scenarios/03_Match_aspect_ratios.png) |
<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\CameraResolution\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run16)
- Project file: `CameraResolution.csproj`


