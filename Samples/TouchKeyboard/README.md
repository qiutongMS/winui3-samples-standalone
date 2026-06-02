# TouchKeyboard — WinUI 3 (migrated from UWP)

**Quality: 94 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 9 / 10 | 8 / 10 | 8 / 10 |

Show / hide soft keyboard, listen for visibility events, switch input scopes (incl. emoji panel).

## Run

```powershell
cd TouchKeyboard && winapp run
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
| **Scenario 1 — Display touch keyboard automatically**<br/>![Scenario 1 — Display touch keyboard automatically](docs/after-scenarios/01_Display_touch_keyboard_automatically.png) | **Scenario 2 — Listen for Show Hide events**<br/>![Scenario 2 — Listen for Show Hide events](docs/after-scenarios/02_Listen_for_Show_Hide_events.png) |
| **Scenario 3 — Programmatically Show Hide the touch keyboard**<br/>![Scenario 3 — Programmatically Show Hide the touch keyboard](docs/after-scenarios/03_Programmatically_Show_Hide_the_touch_keyboard.png) | **Scenario 4 — Showing the Emoji keyboard**<br/>![Scenario 4 — Showing the Emoji keyboard](docs/after-scenarios/04_Showing_the_Emoji_keyboard.png) |

<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\TouchKeyboard\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill
- Project file: `TouchKeyboard\TouchKeyboard.csproj`
