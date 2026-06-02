# winui3-samples-standalone

WinUI 3 desktop ports of nine [Windows-universal-samples](https://github.com/microsoft/Windows-universal-samples) (UWP), produced by the [`winui-uwp-migration`](https://github.com/xiaomgao_microsoft/win-dev-skills-benchmark) agent skill.

Each subdirectory under [`Samples/`](Samples/) is a self-contained WinUI 3 project (`*.csproj`, `*.slnx`, source, assets) that you can open, build, and run independently. Every sample's `README.md` carries a quality score, a before/after main screenshot, and a per-scenario gallery captured by the benchmark.

## Samples

| Sample | What it shows |
|---|---|
| [AdaptiveStreaming](Samples/AdaptiveStreaming) | HLS/DASH adaptive bitrate streaming, custom HTTP headers, live seekable range, ad insertion |
| [ApplicationData](Samples/ApplicationData) | Settings + roaming/local data containers, file APIs |
| [AssociationLaunching](Samples/AssociationLaunching) | File-type / URI launch handlers, target app preferences |
| [BasicInput](Samples/BasicInput) | Pointer, keyboard, gesture, and stylus input fundamentals |
| [CameraProfile](Samples/CameraProfile) | Query a capture device for compatible media-type profiles |
| [CameraResolution](Samples/CameraResolution) | Enumerate and select preview / photo / video resolutions |
| [Package](Samples/Package) | Inspect MSIX package identity, dependencies, and content |
| [TouchKeyboard](Samples/TouchKeyboard) | Show/hide the soft keyboard, input scopes, emoji panel |
| [XamlFocusVisuals](Samples/XamlFocusVisuals) | Keyboard focus visuals, focus engagement, accessibility |

## Build / run

Each sample uses the standard WinUI 3 desktop project shape (WindowsAppSDK + `net10.0-windows10.0.26100.0`, `win-x64`). From a sample directory:

```powershell
winapp run            # build + launch packaged
winapp build          # build only
```

(`winapp` is the helper CLI in the parent benchmark repo. You can equivalently use `dotnet build` / Visual Studio.)

### Prerequisites
- Visual Studio 2026 with the **Windows App SDK C# Templates** workload, or the .NET 10 SDK with the WinAppSDK workloads installed
- Windows 11 with the **Windows App Runtime 2.x** (the projects auto-deploy a matching runtime during the first run)

## How these were generated

The migrations were produced by running [Claude Opus 4.6](https://www.anthropic.com/claude) with the [`winui-uwp-migration`](https://github.com/xiaomgao_microsoft/win-dev-skills-benchmark/tree/main/src/skills/winui-uwp-migration) agent skill on the original UWP source. The skill:

1. Maps UWP namespaces (`Windows.UI.Xaml.*`) onto WinUI 3 (`Microsoft.UI.Xaml.*`).
2. Replaces UWP-only APIs (`Dispatcher`, `CoreWindow`, `GetForCurrentView()`, …) with WinAppSDK equivalents.
3. Neutralizes UWP-only classes such as `RootFrameNavigationHelper` so per-scenario navigation still works.
4. Builds + launches the migrated app and captures per-scenario screenshots for verification.

Per-sample screenshots in each `docs/after-scenarios/` folder come directly from the benchmark's validation step.

## License

[MIT](LICENSE). Original UWP sample code is © Microsoft Corporation and was published under the MIT License at <https://github.com/microsoft/Windows-universal-samples>.
