# Printing — WinUI 3 (migrated from UWP)

**Quality: 94 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 9 / 10 | 8 / 10 | 8 / 10 |

Drive the Windows print pipeline via `PrintManager` and `PrintDocument`. Six scenario pages cover the basics, standard options, custom options, page ranges, photo printing, and disabling the preview pane.

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

### Scenario 1 — Basic

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 1 — Basic](docs/before-scenarios/01_Basic.png) | ![WinUI 3 — Scenario 1 — Basic](docs/after-scenarios/01_Basic.png) |

### Scenario 2 — Standard Options

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 2 — Standard Options](docs/before-scenarios/02_Scenario2StandardOptons.png) | ![WinUI 3 — Scenario 2 — Standard Options](docs/after-scenarios/02_Scenario2StandardOptons.png) |

### Scenario 3 — Custom Options

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 3 — Custom Options](docs/before-scenarios/03_Custom_Options.png) | ![WinUI 3 — Scenario 3 — Custom Options](docs/after-scenarios/03_Custom_Options.png) |

### Scenario 4 — Page Range

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 4 — Page Range](docs/before-scenarios/04_Page_Range.png) | ![WinUI 3 — Scenario 4 — Page Range](docs/after-scenarios/04_Page_Range.png) |

### Scenario 5 — Photos

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 5 — Photos](docs/before-scenarios/05_Photos.png) | ![WinUI 3 — Scenario 5 — Photos](docs/after-scenarios/05_Photos.png) |

### Scenario 6 — Disable Preview

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 6 — Disable Preview](docs/before-scenarios/06_Disable_Preview.png) | ![WinUI 3 — Scenario 6 — Disable Preview](docs/after-scenarios/06_Disable_Preview.png) |
<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\Printing\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run32)
- Project file: `Printing\Printing.csproj`

