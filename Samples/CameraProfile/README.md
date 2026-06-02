# CameraProfile — WinUI 3 (migrated from UWP)

**Quality: 96 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 10 / 10 | 8 / 10 | 9 / 10 |

Query a media capture device for the collection of media types that can work together on a given device (Video Profile).

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
## Per-scenario WinUI 3 output

Each scenario page from the migrated WinUI 3 build:

|  |  |
|:-:|:-:|
| **Scenario 1 — Locate Record Specific Profile**<br/>![Scenario 1 — Locate Record Specific Profile](docs/after-scenarios/01_Locate_Record_Specific_Profile.png) | **Scenario 2 — Query Profile for Concurrency**<br/>![Scenario 2 — Query Profile for Concurrency](docs/after-scenarios/02_Query_Profile_for_Concurrency.png) |
| **Scenario 3 — Query Profile for HDR Support**<br/>![Scenario 3 — Query Profile for HDR Support](docs/after-scenarios/03_Query_Profile_for_HDR_Support.png) |  |

<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\CameraProfile\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run16)
- Project file: `CameraProfile\CameraProfile.csproj`
