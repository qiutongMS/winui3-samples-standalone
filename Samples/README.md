# WinUI 3 sample showcase — migrated from UWP

Eleven UWP samples migrated to WinUI 3 / Windows App SDK by an LLM
agent (Claude Opus 4.6) using the `winui-uwp-migration` skill, then
scored against a behavioral baseline of the original UWP apps.

Each subfolder is a standalone WinUI 3 project — build and launch with
`winapp run`.

## At a glance

| Sample | Quality | Sub-scores (P / U / V / F) | What it demonstrates |
|---|:-:|:-:|---|
| [AdaptiveStreaming](AdaptiveStreaming/README.md) | **94** | 9 / 9 / 8 / 8 | HLS/DASH adaptive bitrate streaming, custom HTTP headers, live seekable range, ad insertion. |
| [ApplicationData](ApplicationData/README.md) | **92** | 9 / 9 / 8 / 10 | Local / roaming / temp data folders, settings persistence, composite settings, versioned schemas. |
| [AssociationLaunching](AssociationLaunching/README.md) | **93** | 9 / 9 / 7 / 8 | URI scheme and file association handlers, launch-for-results, app-to-app activation. |
| [BasicInput](BasicInput/README.md) | **95** | 9 / 9 / 8 / 9 | Pointer, keyboard, and manipulation event routing across nested elements; gestures and tap targets. |
| [CameraProfile](CameraProfile/README.md) | **96** | 9 / 10 / 8 / 9 | Query a media capture device for collections of media types that work together on a given device (Video Profile). |
| [CameraResolution](CameraResolution/README.md) | **93** | 9 / 9 / 8 / 7 | Change the resolution of a capture device's preview, photo, and video streams. |
| [Package](Package/README.md) | **96** | 9 / 9 / 8 / 10 | Read app package identity, version, dependencies, and signature info via Windows.ApplicationModel.Package. |
| [TouchKeyboard](TouchKeyboard/README.md) | **94** | 9 / 9 / 8 / 8 | Show / hide soft keyboard, listen for visibility events, switch input scopes (incl. emoji panel). |
| [XamlFocusVisuals](XamlFocusVisuals/README.md) | **93** | 9 / 8 / 7 / 9 | Keyboard-focus visual customization, system vs custom focus rectangles, reveal animations. |

- **P / U / V / F** = Project structure / UI fidelity / Visual fidelity / Functional fidelity, each 0-10.
- Migrated across three benchmark runs: 3 from run8 (5-28), 1 from run9 (5-28), 7 from run16 (5-29).

## How to run any sample

```powershell
cd Samples\<SampleName>
# If a project subfolder exists (e.g. AdaptiveStreaming\AdaptiveStreaming\),
# cd into it; otherwise the csproj is right here.
winapp run
```

## Comparing UWP source and WinUI 3 migration

Each sample has a `docs/before-main.png` (the original UWP) and a
`docs/after-main.png` (this migration) side-by-side in its README.

For the original UWP sources, see the sibling `uwp-samples-standalone` repo
under `Samples\<SampleName>\cs\`.