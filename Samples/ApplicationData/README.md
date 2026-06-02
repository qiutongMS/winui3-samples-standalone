# ApplicationData — WinUI 3 (migrated from UWP)

**Quality: 92 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 9 / 10 | 8 / 10 | 10 / 10 |

Local / roaming / temp data folders, settings persistence, composite settings, versioned schemas.

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
| **Scenario 1 — Scenario1 Files**<br/>![Scenario 1 — Scenario1 Files](docs/after-scenarios/01_Scenario1_Files.png) | **Scenario 2 — Scenario2 Settings**<br/>![Scenario 2 — Scenario2 Settings](docs/after-scenarios/02_Scenario2_Settings.png) |
| **Scenario 3 — Scenario3 SettingContainer**<br/>![Scenario 3 — Scenario3 SettingContainer](docs/after-scenarios/03_Scenario3_SettingContainer.png) | **Scenario 4 — Scenario4 CompositeSettings**<br/>![Scenario 4 — Scenario4 CompositeSettings](docs/after-scenarios/04_Scenario4_CompositeSettings.png) |
| **Scenario 6 — Clear**<br/>![Scenario 6 — Clear](docs/after-scenarios/06_Clear.png) | **Scenario 7 — SetVersion**<br/>![Scenario 7 — SetVersion](docs/after-scenarios/07_SetVersion.png) |

<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\ApplicationData\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run16)
- Project file: `ApplicationDataSample\ApplicationDataSample.csproj`
