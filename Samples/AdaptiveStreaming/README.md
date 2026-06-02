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
## Per-scenario WinUI 3 output

Each scenario page from the migrated WinUI 3 build:

|  |  |
|:-:|:-:|
| **Scenario 1 — Simplest Adaptive Streaming**<br/>![Scenario 1 — Simplest Adaptive Streaming](docs/after-scenarios/01_Simplest_Adaptive_Streaming.png) | **Scenario 2 — Event Handlers**<br/>![Scenario 2 — Event Handlers](docs/after-scenarios/02_Event_Handlers.png) |
| **Scenario 3 — Network Request Modification**<br/>![Scenario 3 — Network Request Modification](docs/after-scenarios/03_Network_Request_Modification.png) | **Scenario 4 — Adaptive Streaming Tuning**<br/>![Scenario 4 — Adaptive Streaming Tuning](docs/after-scenarios/04_Adaptive_Streaming_Tuning.png) |
| **Scenario 5 — Metadata**<br/>![Scenario 5 — Metadata](docs/after-scenarios/05_Metadata.png) | **Scenario 6 — Ad Insertion**<br/>![Scenario 6 — Ad Insertion](docs/after-scenarios/06_Ad_Insertion.png) |
| **Scenario 7 — Live Seekable Range**<br/>![Scenario 7 — Live Seekable Range](docs/after-scenarios/07_Live_Seekable_Range.png) |  |

<!-- END per-scenario -->

## Source

- UWP source: `..\..\..\uwp-samples-standalone\Samples\AdaptiveStreaming\cs\`
- Migrated by: Claude Opus 4.6 + the `winui-uwp-migration` skill
- Project file: `AdaptiveStreaming\AdaptiveStreaming.csproj`

