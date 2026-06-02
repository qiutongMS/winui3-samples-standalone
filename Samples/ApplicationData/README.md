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
## Per-scenario UWP -> WinUI 3 comparison

Initial state of each scenario page, original UWP sample on the left and the migrated WinUI 3 build on the right.

### Scenario 1 — Files

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| _(no UWP baseline captured)_ | ![WinUI 3 — Scenario 1 — Files](docs/after-scenarios/01_Scenario1_Files.png) |

### Scenario 2 — Settings

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| _(no UWP baseline captured)_ | ![WinUI 3 — Scenario 2 — Settings](docs/after-scenarios/02_Scenario2_Settings.png) |

### Scenario 3 — SettingContainer

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| _(no UWP baseline captured)_ | ![WinUI 3 — Scenario 3 — SettingContainer](docs/after-scenarios/03_Scenario3_SettingContainer.png) |

### Scenario 4 — CompositeSettings

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| _(no UWP baseline captured)_ | ![WinUI 3 — Scenario 4 — CompositeSettings](docs/after-scenarios/04_Scenario4_CompositeSettings.png) |

### Scenario 6 — Clear

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 6 — Clear](docs/before-scenarios/06_Clear.png) | ![WinUI 3 — Scenario 6 — Clear](docs/after-scenarios/06_Clear.png) |

### Scenario 7 — SetVersion

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 7 — SetVersion](docs/before-scenarios/07_SetVersion.png) | ![WinUI 3 — Scenario 7 — SetVersion](docs/after-scenarios/07_SetVersion.png) |
<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\ApplicationData\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run16)
- Project file: `ApplicationDataSample\ApplicationDataSample.csproj`


