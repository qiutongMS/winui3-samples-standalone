# BasicInput — WinUI 3 (migrated from UWP)

**Quality: 95 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 9 / 10 | 8 / 10 | 9 / 10 |

Pointer, keyboard, and manipulation event routing across nested elements; gestures and tap targets.

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

### Scenario 1 — Input Events

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 1 — Input Events](docs/before-scenarios/01_1_Input_Events.png) | ![WinUI 3 — Scenario 1 — Input Events](docs/after-scenarios/01_1_Input_Events.png) |

### Scenario 2 — PointerPoint Properties

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 2 — PointerPoint Properties](docs/before-scenarios/02_2_PointerPoint_Properties.png) | ![WinUI 3 — Scenario 2 — PointerPoint Properties](docs/after-scenarios/02_2_PointerPoint_Properties.png) |

### Scenario 3 — Device Capabilities

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 3 — Device Capabilities](docs/before-scenarios/03_3_Device_Capabilities.png) | ![WinUI 3 — Scenario 3 — Device Capabilities](docs/after-scenarios/03_3_Device_Capabilities.png) |

### Scenario 4 — XAML Manipulations

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 4 — XAML Manipulations](docs/before-scenarios/04_4_XAML_Manipulations.png) | ![WinUI 3 — Scenario 4 — XAML Manipulations](docs/after-scenarios/04_4_XAML_Manipulations.png) |

### Scenario 5 — Gesture Recognizer

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 5 — Gesture Recognizer](docs/before-scenarios/05_5_Gesture_Recognizer.png) | ![WinUI 3 — Scenario 5 — Gesture Recognizer](docs/after-scenarios/05_5_Gesture_Recognizer.png) |
<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\BasicInput\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill (run16)
- Project file: `BasicInput\BasicInput.csproj`

