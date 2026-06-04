# CameraFrames — WinUI 3 (migrated from UWP)

**Quality: 95 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 9 / 10 | 8 / 10 | 9 / 10 |

Pull individual camera frames as `SoftwareBitmap` via `MediaFrameReader` for software processing.

## Run

`powershell
winapp run
`

## Before / after (main view)

UWP source (baseline) and the WinUI 3 migration of the same scenario:

| UWP source | WinUI 3 (this migration) |
|:-:|:-:|
| ![UWP main view](docs/before-main.png) | ![WinUI 3 main view](docs/after-main.png) |

<!-- BEGIN per-scenario -->
## Per-scenario UWP -> WinUI 3 comparison

Initial state of each scenario page, original UWP sample on the left and the migrated WinUI 3 build on the right.

### Scenario 1 — Shared mode access to color depth and infrared frame sources

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| _(no UWP baseline captured)_ | ![WinUI 3 — Scenario 1 — Shared mode access to color depth and infrared frame sources](docs/after-scenarios/01_Shared_mode_access_to_color_depth_and_infrared_frame_sources.png) |

### Scenario 2 — Find and display all media frame sources

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 2 — Find and display all media frame sources](docs/before-scenarios/02_Find_and_display_all_media_frame_sources.png) | ![WinUI 3 — Scenario 2 — Find and display all media frame sources](docs/after-scenarios/02_Find_and_display_all_media_frame_sources.png) |
<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\CameraFrames\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run33)
- Project file: `CameraFrames\CameraFrames.csproj`

