# Roadmap

Estado actual: la app funciona en Linux/macOS/Windows desktop, tiene 3 idiomas, tema claro/oscuro, TTS via `spd-say`/`espeak`, persistencia por idioma, y se puede empaquetar como AppImage.

Las mejoras se ordenan por impacto en el usuario vs. esfuerzo del developer.

## Ahora (1-2 semanas)

### 1. Migrar a MVVM con CommunityToolkit
**Por qué**: el code-behind actual mezcla lógica de UI con lógica de negocio. Cada `View` es un mini view-model + vista. Migrar a `CommunityToolkit.Mvvm` reduce boilerplate y facilita testing.

**Cómo**:
- Añadir `CommunityToolkit.Mvvm` 8.x
- Crear `ViewModels/PracticaPalabras.ViewModels` con `ObservableObject` base
- XAML bindings usan `[ObservableProperty]` + `[RelayCommand]`
- Mantener `Instance` singleton para preservar contratos con código existente

**Esfuerzo**: 2-3 días para 3 ViewModels.

### 2. Buscar palabras en el diccionario
**Por qué**: con 200+ palabras hace falta filtrar.

**Cómo**:
- Añadir `TextBox` con `Watermark="Buscar"` encima del editor
- Filtrar `Text` en tiempo real con `TextChanged` event
- Highligh matching text en el editor

**Esfuerzo**: 2-3 horas.

### 3. Modo revisión de errores
**Por qué**: el usuario se equivoca con palabras, pero no hay manera de repasarlas concentradas.

**Cómo**:
- Persistir `dicWrongWords` y `dicRepeat` en `PreferencesService` (por sesión)
- Nueva `ReviewView` que muestra solo palabras falladas
- Acceso desde el flyout o Configuration

**Esfuerzo**: 1 día.

### 4. Indicador de progreso visual
**Por qué**: el contador `Max=N Current=M` no transmite progreso.

**Cómo**:
- Reemplazar chips por `ProgressBar` Material You (`LinearProgressBar` real)
- Mostrar racha actual de aciertos consecutivos
- Animación al acertar (checkmark breve)

**Esfuerzo**: 4 horas.

## Pronto (1 mes)

### 5. Spaced repetition (SM-2)
**Por qué**: ahora todas las palabras tienen la misma frecuencia. El algoritmo SM-2 prioriza palabras que fallaste y reduce las que dominas.

**Cómo**:
- Añadir campos `easiness`, `interval`, `repetitions` a `Word`
- Implementar `Word.ShouldReviewToday()` basado en `interval` + `lastReview`
- Persistir en `dictionary.txt` (formato `word;clue;easiness;interval;repetitions;lastReview`)
- Migración del formato viejo al nuevo es transparente

**Esfuerzo**: 1 semana.

### 6. Estadísticas de sesión
**Por qué**: el usuario no sabe cuánto ha practicado ni su accuracy.

**Cómo**:
- Persistir por sesión: timestamp inicio, palabras practicadas, aciertos, errores
- Nueva `StatsView` con gráficos simples (rectángulos con `Canvas` o ProgressBars)
- Racha diaria (similar a Duolingo)

**Esfuerzo**: 1 semana.

### 7. Activar target Android
**Por qué**: el csproj ya tiene la config comentada, pero falta probarla en un dispositivo/emulador.

**Cómo**:
- Descomentar las 2 líneas en `csproj`
- `dotnet workload install android`
- Probar en emulador Android 13+
- Ajustar layout para touch (botones más grandes, menos padding)
- Generar APK firmado con `dotnet publish -f net9.0-android -c Release`

**Requisitos**: Android SDK + JDK + teléfono o emulador.

**Esfuerzo**: 1 día si tienes SDK + emulador, 3+ días si tienes que instalar todo.

### 8. Tests unitarios
**Por qué**: hoy un cambio en `Speak.cs` puede romper la lógica de pronunciación silenciosamente.

**Cómo**:
- Crear `PracticaPalabras.Tests` (xUnit)
- Cubrir: `Word.FromLine` / `ToSaveString`, `Speak.ToHidden`, `Speak.Read` (sin TTS real), `DictionaryView.Clear`, `Language` mapping
- CI con GitHub Actions ejecutando `dotnet test`

**Esfuerzo**: 3-4 días.

## Después (3-6 meses)

### 9. Importar/exportar diccionario
**Por qué**: los usuarios querrán compartir listas o cargar paquetes pre-hechos.

**Cómo**:
- Botón "Export" → escribe JSON o CSV a `~/Downloads/words-<lang>.txt`
- Botón "Import" → abre `StorageProvider.OpenFilePickerAsync`
- Formato con metadatos: `word;clue;tags;difficulty`

**Esfuerzo**: 2-3 días.

### 10. Categorías / temas
**Por qué**: "animals", "food", "verbs" — el usuario puede practicar un tema concreto.

**Cómo**:
- Header `# animal` en el archivo de diccionario como separador de categoría
- Filtro dropdown en `MainView` para practicar solo una categoría
- Persistir última categoría seleccionada

**Esfuerzo**: 1 semana.

### 11. Empaquetado multiplataforma
**Por qué**: AppImage está bien, pero Windows y macOS no tienen un flujo equivalente.

**Cómo**:
- Windows: `dotnet publish -f net9.0-windows10.0.19041.0 -c Release` + crear instalador MSIX con `wix`
- macOS: `dotnet publish -f net9.0-osx -c Release` + `appdmg` para `.dmg`
- Linux: también Flatpak + Snap

**Esfuerzo**: 1-2 semanas total.

### 12. Input por voz
**Por qué**: el usuario puede pronunciar la palabra en lugar de escribirla.

**Cómo**:
- `ISpeechService` ya tiene site para añadir `Listen`
- Linux: `Vosk` (offline) o `Whisper.cpp` (más pesado pero preciso)
- Mostrar transcripción en tiempo real debajo del editor
- Validar contra la palabra objetivo

**Complejidad**: alta (modelos de ML, permisos de micrófono).

**Esfuerzo**: 2-3 semanas.

## Soñado (sin fecha)

- **Cloud sync** — Supabase/Firebase para sync entre dispositivos (privacy-first, opcional)
- **Multiplayer** — competir con amigos en tiempo real (overkill para app de vocabulario)
- **Definiciones API** — buscar definición real en lugar de pista libre
- **Voice cloning** — usar la voz del usuario para TTS
- **WebGPU rendering** — sustituir Canvas si Avalonia 13+ lo soporta bien
- **Plugin system** — juegos de práctica alternativos (memoria, arrastrar y soltar)

## Issues activos

- [ ] `es-ES.xaml.cs` y `es-CA.xaml.cs` originales (en MAUI) tienen caracteres `�` donde iban acentos (encoding bug en el repo original). Si se quiere añadir/quitar dígrafos de pronunciación, re-encodificar desde el código fuente correcto.
- [ ] MAUI original tiene `WebUtility.UrlDecode` para el query string. En Avalonia port, el `word` se pasa por setter directo (`view.SetWord(...)`), no hay decoding. No es bug pero hay que tener cuidado si en el futuro se usa navegación URL-based.

## Contribuir

Sin convenciones todavía. Si se hacen PRs grandes se puede migrar a:
- Branch por feature
- Conventional Commits
- CHANGELOG.md auto-generado con `git-cliff`
