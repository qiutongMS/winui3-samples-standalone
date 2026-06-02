# AdaptiveStreaming — WinUI 3 (migrated from UWP)

**Quality: 94 / 100**

| Project | UI fidelity | Visual fidelity | Functional fidelity |
|:-:|:-:|:-:|:-:|
| 9 / 10 | 9 / 10 | 8 / 10 | 8 / 10 |

HLS/DASH adaptive bitrate streaming, custom HTTP headers, live seekable range, ad insertion.

## Run

```powershell
cd AdaptiveStreaming && winapp run
```

## Before / after (main view)

UWP source (baseline) and the WinUI 3 migration of the same scenario:

| UWP source | WinUI 3 (this migration) |
|:-:|:-:|
| ![UWP main view](docs/before-main.png) | ![WinUI 3 main view](docs/after-main.png) |

<!-- BEGIN per-scenario -->
## Per-scenario UWP -> WinUI 3 comparison

Initial state of each scenario page, original UWP sample on the left and the migrated WinUI 3 build on the right.

### Scenario 1 — Simplest Adaptive Streaming

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 1 — Simplest Adaptive Streaming](docs/before-scenarios/01_Simplest_Adaptive_Streaming.png) | ![WinUI 3 — Scenario 1 — Simplest Adaptive Streaming](docs/after-scenarios/01_Simplest_Adaptive_Streaming.png) |

### Scenario 2 — Event Handlers

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 2 — Event Handlers](docs/before-scenarios/02_Event_Handlers.png) | ![WinUI 3 — Scenario 2 — Event Handlers](docs/after-scenarios/02_Event_Handlers.png) |

### Scenario 3 — Network Request Modification

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 3 — Network Request Modification](docs/before-scenarios/03_Network_Request_Modification.png) | ![WinUI 3 — Scenario 3 — Network Request Modification](docs/after-scenarios/03_Network_Request_Modification.png) |

### Scenario 4 — Adaptive Streaming Tuning

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 4 — Adaptive Streaming Tuning](docs/before-scenarios/04_Adaptive_Streaming_Tuning.png) | ![WinUI 3 — Scenario 4 — Adaptive Streaming Tuning](docs/after-scenarios/04_Adaptive_Streaming_Tuning.png) |

### Scenario 5 — Metadata

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 5 — Metadata](docs/before-scenarios/05_Metadata.png) | ![WinUI 3 — Scenario 5 — Metadata](docs/after-scenarios/05_Metadata.png) |

### Scenario 6 — Ad Insertion

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 6 — Ad Insertion](docs/before-scenarios/06_Ad_Insertion.png) | ![WinUI 3 — Scenario 6 — Ad Insertion](docs/after-scenarios/06_Ad_Insertion.png) |

### Scenario 7 — Live Seekable Range

| UWP source | WinUI 3 (migrated) |
|:-:|:-:|
| ![UWP — Scenario 7 — Live Seekable Range](docs/before-scenarios/07_Live_Seekable_Range.png) | ![WinUI 3 — Scenario 7 — Live Seekable Range](docs/after-scenarios/07_Live_Seekable_Range.png) |
<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\AdaptiveStreaming\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill
- Project file: `AdaptiveStreaming\AdaptiveStreaming.csproj`



