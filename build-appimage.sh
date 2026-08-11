#!/usr/bin/env bash
set -euo pipefail

APP_NAME="PracticaPalabras"
APP_DISPLAY="Practica Palabras"
APP_ID="com.gabrielcatdeveloper.practicapalabras"
VERSION="1.2.0"
ARCH=$(uname -m)
DOTNET_ARCH=$(uname -m | sed -e 's/x86_64/x64/' -e 's/aarch64/arm64/' -e 's/armv7l/arm/')
PUBLISH_DIR="publish/linux-$DOTNET_ARCH"
APPDIR="PracticaPalabras.AppDir"
TOOL="appimagetool-${DOTNET_ARCH}.AppImage"
OUTPUT="${APP_NAME}-${VERSION}-${ARCH}.AppImage"

echo "== Build target: linux-$ARCH (RID: linux-$DOTNET_ARCH) =="

cd "$(dirname "$0")"

echo "== 1) Publish self-contained single-file =="
dotnet publish -c Release -r linux-$DOTNET_ARCH \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:PublishReadyToRun=false \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:DebugType=embedded \
    -o "$PUBLISH_DIR"

EXE="$PUBLISH_DIR/$APP_NAME"
if [ ! -f "$EXE" ]; then
    echo "ERROR: $EXE no existe tras publish"
    exit 1
fi
chmod +x "$EXE"

echo "== 2) Build AppDir =="
rm -rf "$APPDIR"
mkdir -p "$APPDIR/usr/bin"
mkdir -p "$APPDIR/usr/share/applications"
mkdir -p "$APPDIR/usr/share/icons/hicolor/256x256/apps"
mkdir -p "$APPDIR/usr/share/metainfo"

cp "$EXE" "$APPDIR/usr/bin/$APP_NAME"
chmod +x "$APPDIR/usr/bin/$APP_NAME"

cat > "$APPDIR/usr/share/applications/$APP_ID.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=$APP_DISPLAY
Comment=Vocabulary spelling practice
Exec=$APP_NAME
Icon=$APP_ID
Terminal=false
Categories=Education;Languages;
StartupNotify=true
EOF

cat > "$APPDIR/AppRun" <<EOF
#!/usr/bin/env bash
HERE="\$(dirname "\$(readlink -f "\$0")")"
export DOTNET_ROOT="\$HERE/usr/lib/\$APP_NAME"
exec "\$HERE/usr/bin/$APP_NAME" "\$@"
EOF
chmod +x "$APPDIR/AppRun"

cp "$APPDIR/usr/share/applications/$APP_ID.desktop" "$APPDIR/"
if [ -f "Resources/AppIcon/appicon.png" ]; then
    cp "Resources/AppIcon/appicon.png" "$APPDIR/$APP_ID.png"
    cp "Resources/AppIcon/appicon.png" "$APPDIR/usr/share/icons/hicolor/256x256/apps/$APP_ID.png"
elif [ -f "Resources/AppIcon/appicon.svg" ]; then
    cp "Resources/AppIcon/appicon.svg" "$APPDIR/$APP_ID.svg"
    cp "Resources/AppIcon/appicon.svg" "$APPDIR/usr/share/icons/hicolor/256x256/apps/$APP_ID.svg"
fi

cat > "$APPDIR/usr/share/metainfo/$APP_ID.appdata.xml" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<component type="desktop-application">
  <id>$APP_ID</id>
  <name>$APP_DISPLAY</name>
  <summary>Vocabulary spelling practice</summary>
  <metadata_license>CC0-1.0</metadata_license>
  <developer_name>GabrielCatDeveloper</developer_name>
  <categories><category>Education</category></categories>
</component>
EOF

echo "== 3) Download appimagetool =="
if [ ! -f "$TOOL" ]; then
    wget -q "https://github.com/AppImageCommunity/AppImageKit/releases/download/continuous/$TOOL"
    chmod +x "$TOOL"
fi

echo "== 4) Build AppImage =="
rm -f "$OUTPUT"
"./$TOOL" "$APPDIR" "$OUTPUT" 2>&1 | grep -v "WARNING" || true
chmod +x "$OUTPUT" 2>/dev/null || true

if [ -f "$OUTPUT" ]; then
    SIZE=$(du -h "$OUTPUT" | cut -f1)
    echo ""
    echo "OK: $OUTPUT ($SIZE)"
    echo ""
    echo "Run: ./$OUTPUT"
else
    echo ""
    echo "ERROR: AppImage no se generó. Comprueba errores arriba."
    exit 1
fi
