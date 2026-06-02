# Package — WinUI 3 (migrated from UWP)

**Quality: 96 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 9 / 10 | 8 / 10 | 10 / 10 |

Read app package identity, version, dependencies, and signature info via
`Windows.ApplicationModel.Package`. Three scenario pages: Identity,
Installed Location, and Dependencies.

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
| **Scenario 1 — scenario1 identity**<br/>![Scenario 1 — scenario1 identity](docs/after-scenarios/01_scenario1_identity.png) | **Scenario 2 — scenario2 installedlocation**<br/>![Scenario 2 — scenario2 installedlocation](docs/after-scenarios/02_scenario2_installedlocation.png) |
| **Scenario 3 — scenario3 dependencies**<br/>![Scenario 3 — scenario3 dependencies](docs/after-scenarios/03_scenario3_dependencies.png) |  |

<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\Package\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run9)
- Project file: `PackageSample\PackageSample.csproj`

