# AssociationLaunching — WinUI 3 (migrated from UWP)

**Quality: 93 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 9 / 10 | 7 / 10 | 8 / 10 |

URI scheme and file association handlers, launch-for-results, app-to-app activation.

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

### Scenario 1 — Launching a file

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 1 — Launching a file](docs/before-scenarios/01_Launching_a_file.png) | ![WinUI 3 — Scenario 1 — Launching a file](docs/after-scenarios/01_Launching_a_file.png) |

### Scenario 2 — Launching a URI

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 2 — Launching a URI](docs/before-scenarios/02_Launching_a_URI.png) | ![WinUI 3 — Scenario 2 — Launching a URI](docs/after-scenarios/02_Launching_a_URI.png) |

### Scenario 3 — Receiving a file

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 3 — Receiving a file](docs/before-scenarios/03_Receiving_a_file.png) | ![WinUI 3 — Scenario 3 — Receiving a file](docs/after-scenarios/03_Receiving_a_file.png) |

### Scenario 4 — Receiving a URI

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 4 — Receiving a URI](docs/before-scenarios/04_Receiving_a_URI.png) | ![WinUI 3 — Scenario 4 — Receiving a URI](docs/after-scenarios/04_Receiving_a_URI.png) |
<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\AssociationLaunching\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run16)
- Project file: `AssociationLaunching\AssociationLaunching.csproj`


