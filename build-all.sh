#!/usr/bin/env bash
# build-all.sh — Compila PracticaPalabras para Linux + Windows (Android opcional)
# Uso:
#   ./build-all.sh                  # solo Linux + Windows
#   INCLUDE_ANDROID=1 ./build-all.sh # también Android (requiere ./install-prereqs.sh)
set -euo pipefail

cd "$(dirname "$0")"

DIST="dist"
LINUX_ARCH=$(uname -m | sed -e 's/x86_64/x64/' -e 's/aarch64/arm64/' -e 's/armv7l/arm/')
LINUX_UNAME=$(uname -m)
VERSION=$(grep -oP '(?<=<ApplicationDisplayVersion>)[^<]+' PracticaPalabras.Avalonia.csproj 2>/dev/null || true)
if [ -z "$VERSION" ]; then
    VERSION=$(grep -oP '(?<=<PackageVersion>)[^<]+' PracticaPalabras.Avalonia.csproj 2>/dev/null || true)
fi
if [ -z "$VERSION" ]; then
    VERSION=$(grep -oP '(?<=<AssemblyVersion>)[^<]+' PracticaPalabras.Avalonia.csproj 2>/dev/null || true)
fi
if [ -z "$VERSION" ]; then
    VERSION="1.0.0"
fi

mkdir -p "$DIST/linux-$LINUX_ARCH" "$DIST/win-x64" "$DIST/android"

publish() {
    local rid="$1"
    local label="$2"
    local extra_args="${3:-}"
    echo ""
    echo "== $label =="
    local out="$DIST/$label"
    rm -rf "$out"
    dotnet publish -c Release -r "$rid" -f net9.0 \
        --self-contained true \
        -p:PublishSingleFile=true \
        -p:PublishReadyToRun=false \
        -p:IncludeNativeLibrariesForSelfExtract=true \
        -p:DebugType=embedded \
        $extra_args \
        -o "$out"
}

echo "== Version: $VERSION =="
echo "== Host: $(uname -s)/$LINUX_UNAME =="

echo ""
echo "== Linux x64 =="
publish "linux-$LINUX_ARCH" "linux-$LINUX_ARCH"
mv "$DIST/linux-$LINUX_ARCH/PracticaPalabras" "$DIST/PracticaPalabras-linux-$LINUX_UNAME"
chmod +x "$DIST/PracticaPalabras-linux-$LINUX_UNAME"

echo ""
echo "== Windows x64 =="
publish "win-x64" "win-x64"
mv "$DIST/win-x64/PracticaPalabras.exe" "$DIST/PracticaPalabras-windows-x64.exe"

if [ "${INCLUDE_ANDROID:-0}" -eq 1 ]; then
    ANDROID_DOTNET=""
    for cand in "$HOME/.dotnet-android/dotnet" "$HOME/.dotnet/dotnet" "/tmp/opencode/dotnet/dotnet"; do
        if [ -x "$cand" ] && "$cand" workload list 2>/dev/null | grep -q "android"; then
            ANDROID_DOTNET="$cand"
            break
        fi
    done

    if [ -n "$ANDROID_DOTNET" ]; then
        ANDROID_HOME="${ANDROID_HOME:-}"
        for sdk in "$ANDROID_HOME" "$HOME/android-sdk" "/opt/android-sdk" "/usr/lib/android-sdk"; do
            if [ -n "$sdk" ] && [ -d "$sdk/platforms" ]; then
                export ANDROID_HOME="$sdk"
                break
            fi
        done
        for jdk in "$HOME/.jdk-17" /usr/lib/jvm/java-17-openjdk-amd64 /usr/lib/jvm/temurin-17-jdk; do
            if [ -x "$jdk/bin/javac" ]; then
                export JAVA_HOME="$jdk"
                break
            fi
        done
        if [ -z "${ANDROID_HOME:-}" ] || [ ! -d "$ANDROID_HOME/platforms" ]; then
            echo ""
            echo "== Android =="
            echo "SKIP: Android SDK no encontrado. Ejecuta ./install-prereqs.sh primero."
            echo "Esperado: \$HOME/android-sdk con /platforms/android-34.0/"
        else
            echo ""
            echo "== Android (unsigned APK) =="
            echo "ANDROID_HOME=$ANDROID_HOME"
            echo "JAVA_HOME=$JAVA_HOME"
            DOTNET_ROOT="$(dirname "$ANDROID_DOTNET")" \
            "$ANDROID_DOTNET" restore -p:IncludeAndroid=true 2>&1 | tail -3
            if DOTNET_ROOT="$(dirname "$ANDROID_DOTNET")" \
               "$ANDROID_DOTNET" msbuild -t:SignAndroidPackage \
                   -p:AndroidKeyStore=false \
                   -p:AndroidEnableAssemblyCompression=false \
                   -p:IncludeAndroid=true \
                   -p:TargetFramework=net9.0-android \
                   -p:Configuration=Release 2>&1 | tail -10; then
                :
            else
                echo ""
                echo "WARN: Android build falló (tooling .NET 9 + SDK 35.0.7 bug)."
            fi

            APK=$(find /home/gabriel/Source/PracticaPalabras/port -name "*-Signed.apk" 2>/dev/null | head -1)
            if [ -z "$APK" ]; then
                APK=$(find /home/gabriel/Source/PracticaPalabras/port -name "*.apk" -not -name "*Unsigned*" 2>/dev/null | head -1)
            fi
            if [ -n "$APK" ]; then
                cp "$APK" "$DIST/PracticaPalabras-android.apk"
                echo "OK: APK generado en $APK"
            fi
        fi
    else
        echo ""
        echo "== Android =="
        echo "SKIP: Android workload no instalado. Ejecuta ./install-prereqs.sh primero."
    fi
fi

echo ""
echo "== Salidas =="
ls -lh "$DIST" | grep -v "^d" | grep -v "^total"
for d in "$DIST"/*/; do
    [ -d "$d" ] && rm -rf "$d"
done

echo ""
echo "✓ Build completo. Archivos en $DIST/"
