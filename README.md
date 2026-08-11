# Practica Palabras — Avalonia Port

A multi-platform port of the original .NET MAUI app to [Avalonia UI 12](https://avaloniaui.net). Targets `net9.0` and runs natively on **Linux**, macOS, Windows, and Browser (with extra configuration).

The original MAUI app targets `net9.0-android` and `net9.0-windows10.0.19041.0` only and cannot be previewed on Linux. This port exists so the same vocabulary-practice UX can be run on a Linux desktop.

Ver [ROADMAP.md](ROADMAP.md) para próximos pasos.

## Quick start

```bash
cd port
dotnet run
```

That's it. No platform-specific workloads needed — the same binary runs on Linux, macOS, and Windows.

### Build a portable single-file binary

```bash
dotnet publish -c Release -r linux-x64 --self-contained true \
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
    -o publish/linux-x64
```

Output: `publish/linux-x64/PracticaPalabras` (~95 MB). Runs on any Linux x64 with glibc ≥ 2.31, no .NET install needed. For macOS replace `linux-x64` with `osx-arm64` / `osx-x64`; for Windows with `win-x64`.

### Build an AppImage (Linux)

```bash
./build-appimage.sh
```

The script publishes self-contained, builds the AppDir (with `.desktop`, metainfo, AppRun), downloads `appimagetool` if missing, and outputs `PracticaPalabras-<version>-<arch>.AppImage`. Requires `wget` for the first run.

### Build for Linux + Windows

```bash
./build-all.sh
```

Output en `dist/`:
- `PracticaPalabras-linux-x86_64` (~89 MB, ejecutable Linux)
- `PracticaPalabras-windows-x64.exe` (~93 MB, ejecutable Windows)

Ambos self-contained, sin .NET instalado en la máquina destino.

### Build con Android (opcional, experimental)

> ⚠️ El tooling Android de .NET 9/10 con Avalonia tiene bugs conocidos. **Linux + Windows son los targets soportados**. Android queda pendiente hasta que Microsoft arregle el packaging.

Si quieres intentarlo:

```bash
./install-prereqs.sh   # ~2 GB de dependencias: .NET 9 standalone, Android workload, JDK 17, Android SDK
INCLUDE_ANDROID=1 ./build-all.sh
```

Variables:
- `INCLUDE_ANDROID=1 ./build-all.sh` — añade Android al build (off por defecto)
- `SKIP_DOTNET=1 ./install-prereqs.sh` — asume .NET 9 standalone ya instalado, solo JDK + Android SDK

Problemas conocidos:
- Avalonia 12.x requiere .NET 10 para Android, pero .NET 10 SDK 10.0.302 tiene un bug buscando paquetes `Mono.linux-x64` que no existen
- Avalonia 11.3.20 (último con .NET 9) + Android SDK 35.0.7 falla en `SignAndroidPackage` con "Assembly store generator did not generate any stores"

### SDK requirement

Requires the **.NET 9 SDK** (matches the original MAUI project's target). Install it with:

```bash
curl -fsSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 9.0
```

To downgrade the target to `net6.0` or `net8.0`, edit `<TargetFramework>` and the `Avalonia.*` package versions in `PracticaPalabras.Avalonia.csproj`. Note: Avalonia 12.x is the first version with `net6.0` support being deprecated; use Avalonia 11.3.x for `net6.0`.

### Optional: install TTS for character-by-character pronunciation

The app reads each character of the active word aloud. By default it falls back to console logging. To get real audio on Linux:

```bash
# Debian/Ubuntu
sudo apt install espeak-ng

# Fedora
sudo dnf install espeak-ng

# Arch
sudo pacman -S espeak-ng
```

`SpeechService` (`port/Services/SpeechService.cs`) auto-detects `espeak` on first use.

## What was ported

| MAUI (original) | Avalonia (this port) |
| --- | --- |
| `MauiApp.CreateBuilder()` + `App.xaml` | `AppBuilder.Configure<App>()` + `App.axaml` (`Program.cs:5-13`) |
| `Shell` w/ flyout + routes | `Window` + `SplitView` (`ShellWindow.axaml`) |
| `ContentPage` per route | `UserControl` per view, swapped in `ContentControl` |
| `AppThemeBinding Light=..., Dark=...` | `<ResourceDictionary.ThemeDictionaries>` in `Styles/Colors.axaml` |
| `Microsoft.Maui.Storage.Preferences` | `Services/PreferencesService.cs` (JSON in `LocalApplicationData/PracticaPalabras/preferences.json`) |
| `Microsoft.Maui.Media.TextToSpeech` | `Services/ISpeechService.cs` + `SpeechService.cs` (uses `espeak`, falls back to logging) |
| `Languages/*.xaml` registered as `MauiAsset` w/ code-behind per lang | `Languages/*.axaml` plain resource dictionaries, loaded via `AvaloniaXamlLoader.Load(uri)` (`App.axaml.cs:46-53`); complex-pronuntiation logic moved into `Language.ConfigSpeak(speak)` |
| `Routing.RegisterRoute(...)` + `Shell.Current.GoToAsync("route?key=value")` | `ShellWindow.Instance.NavigateTo<T>()` / `NavigateTo(view)` (`ShellWindow.axaml.cs:42-50`); view params set via public methods |
| `Shell.FlyoutFooter` with dynamically-built `Label`s | Pane rebuilt in code (`ShellWindow.axaml.cs:74-83`) using `Border` chips with theme-aware brushes |
| `ConfigurationPage.Instance` singleton | `ConfigurationView.Instance` singleton — same pattern |
| `Editor` + `TextChanged` w/ Enter-detection | `TextBox` + `KeyDown` event (`MainView.axaml.cs:228-233`) |

## What's identical

- Domain models (`Models/Word.cs`)
- `Speak` business logic (now talks to `ISpeechService` instead of MAUI's TextToSpeech)
- Language string dictionaries (`Languages/*.axaml`) — same keys, same strings, no code-behind
- Dictionary file format (`word;clue` per line, `Word.ToSaveString` / `Word.FromLine`)
- Per-language dictionary at `LocalApplicationData/<LangCode>/dictionary.txt`
- All gameplay logic in `MainView` (repetition, wrong-word tracking, "imagine" threshold, navigation to `VisualitzationWordView`)

## Theme tokens

`Styles/Colors.axaml` exposes a Material You palette as **theme dictionaries**. Resources defined under `<ResourceDictionary x:Key="Light">` and `<ResourceDictionary x:Key="Dark">` are auto-swapped by Avalonia based on `Application.Current.RequestedThemeVariant` (or system theme).

Style classes available in `Styles/Styles.axaml`:

- Layout: `card`, `card-high`, `card-hero`, `card-error`, `input-field`, `setting-card`
- Chips: `chip-primary`, `chip-secondary`, `lang-chip`, `lang-chip-active`
- Text: `caption`, `section-label`, `headline-large`, `body-large`
- Buttons: `Button` (filled tonal), `Button.nav` (transparent nav row)

To switch themes programmatically:

```csharp
Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
```

## Layout

```
port/
├── Program.cs                  # entry point + AppBuilder
├── App.axaml(.cs)              # App + theme + language dictionary loader
├── ShellWindow.axaml(.cs)      # main window + SplitView flyout
├── Models/Word.cs
├── Services/
│   ├── Language.cs             # static class + per-language ConfigSpeak
│   ├── Speak.cs                # Read + IReadController
│   ├── ISpeechService.cs
│   ├── SpeechService.cs        # espeak via Process, falls back to logging
│   └── PreferencesService.cs   # JSON-backed replacement for MAUI Preferences
├── Views/
│   ├── MainView.axaml(.cs)
│   ├── DictionaryView.axaml(.cs)
│   ├── ConfigurationView.axaml(.cs)
│   └── VisualitzationWordView.axaml(.cs)
├── Styles/
│   ├── Colors.axaml            # theme dictionaries (Light/Dark)
│   └── Styles.axaml            # control styles + reusable classes
└── Languages/
    ├── en-UK.axaml
    ├── es-ES.axaml
    └── es-CA.axaml
```

## Differences from MAUI original

- **Navigation is imperative**, not route-based. There are no query strings — view parameters are passed by calling setters or constructors before `NavigateTo(view)`.
- **No `Editor`**: Avalonia's `TextBox` with `AcceptsReturn="True"` is the multi-line equivalent. The "Press ENTER to submit" behavior on `MainView` is handled via `KeyDown` instead of `TextChanged` detecting a trailing newline — more reliable across platforms.
- **`Speak.Read` no longer aborts via a `Can` boolean flag on the page**: it uses `CancellationToken` from a `CancellationTokenSource` that the view disposes on detach (`VisualitzationWordView.axaml.cs:46-51`).
- **Static language resource dictionaries**: the language `.axaml` files are plain resources (no `x:Class`, no code-behind). The pronunciation setup that MAUI put in `ExtensionEng`/`ExtensionEsp`/`ExtensionCat` now lives in `Language.ConfigEsp`/`ConfigCat` (`Services/Language.cs:60-99`).
- **`VisualitzationWordView` constructor no longer parses query-string params**: it exposes `SetWord(string)` which `MainView` calls before navigating.

## Known limitations

- TTS quality depends on `espeak` voices installed on the host. Catalan/Spanish voice coverage is usually good; check with `espeak --voices=es` and `espeak --voices=ca`.
- No tray icon, no splash screen, no window-state persistence beyond default — Avalonia equivalent would need extra code.
- Browser target (`net9.0-browser` via `Avalonia.Browser`) is not configured here; add `<ProjectReference Include="..." />` to an `Avalonia.Browser` head project if needed.