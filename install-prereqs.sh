#!/usr/bin/env bash
# install-prereqs.sh — Instala todo lo necesario para compilar la app en Linux + Windows + Android
set -euo pipefail

cd "$(dirname "$0")"

if [ "$(id -u)" -eq 0 ]; then
    SUDO=""
else
    SUDO="sudo"
fi

DOTNET_TEMP_DIR="$HOME/.dotnet-temp"
mkdir -p "$DOTNET_TEMP_DIR"
export DOTNET_CLI_TEMP_DIRECTORY="$DOTNET_TEMP_DIR"
export TMPDIR="$DOTNET_TEMP_DIR"
echo "==> dotnet temp: $DOTNET_CLI_TEMP_DIRECTORY (= workaround para /tmp tmpfs pequeño)"

if command -v apt-get >/dev/null 2>&1; then
    PKG_MGR="apt"
    PKG_UPDATE="$SUDO apt-get update"
    PKG_INSTALL="$SUDO apt-get install -y"
elif command -v dnf >/dev/null 2>&1; then
    PKG_MGR="dnf"
    PKG_UPDATE="true"
    PKG_INSTALL="$SUDO dnf install -y"
elif command -v pacman >/dev/null 2>&1; then
    PKG_MGR="pacman"
    PKG_UPDATE="$SUDO pacman -Sy"
    PKG_INSTALL="$SUDO pacman -S --noconfirm"
else
    echo "ERROR: gestor de paquetes no soportado (apt/dnf/pacman)"
    exit 1
fi

echo "== Gestor de paquetes detectado: $PKG_MGR =="

if [ "${SKIP_DOTNET:-0}" -ne 1 ]; then
    echo ""
    echo "== 1) .NET 9 SDK (instalación limpia standalone) =="
    if ! command -v dotnet >/dev/null 2>&1; then
        HAS_DOTNET=0
    else
        HAS_DOTNET=$(dotnet --list-sdks 2>/dev/null | grep -c "^9\." || true)
    fi

    if [ "$HAS_DOTNET" -eq 0 ]; then
        echo "Instalando .NET 9 SDK en \$HOME/.dotnet..."
        mkdir -p /tmp/opencode
        curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/opencode/dotnet-install.sh
        chmod +x /tmp/opencode/dotnet-install.sh
        /tmp/opencode/dotnet-install.sh --channel 9.0 --install-dir "$HOME/.dotnet"

        if ! grep -q "HOME/.dotnet" "$HOME/.bashrc" 2>/dev/null; then
            echo 'export PATH="$HOME/.dotnet:$PATH"' >> "$HOME/.bashrc"
            echo 'export DOTNET_ROOT="$HOME/.dotnet"' >> "$HOME/.bashrc"
        fi
        export PATH="$HOME/.dotnet:$PATH"
        export DOTNET_ROOT="$HOME/.dotnet"
    fi

    FRESH_DOTNET="$HOME/.dotnet-android"
    if [ ! -d "$FRESH_DOTNET/sdk/9.0.316" ]; then
        echo "Instalando .NET 9 SDK fresh en $FRESH_DOTNET (sin .NET 6)..."
        mkdir -p /tmp/opencode
        curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/opencode/dotnet-install.sh
        chmod +x /tmp/opencode/dotnet-install.sh
        /tmp/opencode/dotnet-install.sh --channel 9.0 --install-dir "$FRESH_DOTNET"
    fi

    DOTNET9_BIN="$FRESH_DOTNET/dotnet"
    echo "OK: SDK 9 fresh = $($DOTNET9_BIN --version) en $DOTNET9_BIN"
    echo "(standalone sin .NET 6, en $FRESH_DOTNET)"
fi

if [ "${SKIP_ANDROID:-0}" -ne 1 ]; then
    echo ""
    echo "== 2) Android workload (con .NET 9 standalone) =="
    if ! "$DOTNET9_BIN" workload list 2>/dev/null | grep -q "android"; then
        echo "Instalando Android workload en $FRESH_DOTNET..."

        HIDE_LIST=()
        for dir in "$HOME/.dotnet/sdk-manifests"/6.*; do
            if [ -d "$dir" ] && [ "${dir##*/}" != "${dir##*/}.bak" ]; then
                mv "$dir" "${dir}.bak"
                HIDE_LIST+=("${dir}.bak")
            fi
        done
        [ ${#HIDE_LIST[@]} -gt 0 ] && echo "(ocultando manifests de .NET 6: ${HIDE_LIST[*]})"

        trap '
            for d in "${HIDE_LIST[@]}"; do
                [ -d "$d" ] && mv "$d" "${d%.bak}"
            done
        ' EXIT

        DOTNET_CLI_TEMP_DIRECTORY="$DOTNET_TEMP_DIR" \
        NUGET_PACKAGES="$HOME/.nuget-cache" \
        DOTNET_ROOT="$FRESH_DOTNET" \
        "$DOTNET9_BIN" workload install android --skip-manifest-update --skip-sign-check --verbosity detailed
        WORKLOAD_RC=$?

        for d in "${HIDE_LIST[@]}"; do
            [ -d "$d" ] && mv "$d" "${d%.bak}"
        done
        trap - EXIT

        if [ "$WORKLOAD_RC" -ne 0 ]; then
            exit "$WORKLOAD_RC"
        fi
    fi
    echo "OK: $($DOTNET9_BIN workload list | grep android || true)"
    echo "(workload instalado en $FRESH_DOTNET/sdk/ y $FRESH_DOTNET/packs/)"

    echo ""
    echo "== 3) JDK 17+ (prueba varias versiones) =="
    NEED_JDK=0
    if ! command -v javac >/dev/null 2>&1; then
        NEED_JDK=1
    else
        JDK_VER=$(javac -version 2>&1 | awk '{print $2}' | cut -d. -f1)
        if [ "$JDK_VER" -lt 17 ] 2>/dev/null; then
            NEED_JDK=1
        fi
    fi

    if [ "$NEED_JDK" -eq 1 ]; then
        echo "Buscando JDK disponible..."
        JDK_PKG=""
        for pkg in openjdk-21-jdk openjdk-17-jdk openjdk-19-jdk openjdk-11-jdk openjdk-25-jdk; do
            if apt-cache show "$pkg" >/dev/null 2>&1 || dnf info "$pkg" >/dev/null 2>&1; then
                JDK_PKG="$pkg"
                echo "Seleccionado: $pkg"
                break
            fi
        done

        if [ -z "$JDK_PKG" ]; then
            echo "No hay openjdk en repos. Descargando Eclipse Temurin 17 a $HOME/.jdk-17..."
            mkdir -p "$HOME/.jdk-17"
            cd /tmp/opencode
            wget -q "https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.13%2B11/OpenJDK17U-jdk_x64_linux_hotspot_17.0.13_11.tar.gz" -O temurin17.tar.gz
            tar -xzf temurin17.tar.gz -C "$HOME/.jdk-17" --strip-components=1
            rm -f temurin17.tar.gz
            echo "export JAVA_HOME=$HOME/.jdk-17" >> "$HOME/.bashrc"
            echo "export PATH=\$JAVA_HOME/bin:\$PATH" >> "$HOME/.bashrc"
            export JAVA_HOME="$HOME/.jdk-17"
            export PATH="$JAVA_HOME/bin:$PATH"
        else
            echo "Instalando $JDK_PKG..."
            case "$PKG_MGR" in
                apt)    $PKG_UPDATE && $PKG_INSTALL "$JDK_PKG" unzip wget ;;
                dnf)    $PKG_INSTALL "$(echo $JDK_PKG | sed 's/openjdk/java/g; s/-jdk/-openjdk-devel/g')" unzip wget ;;
                pacman) $PKG_UPDATE && $PKG_INSTALL jdk-openjdk unzip wget ;;
            esac
        fi
    fi
    echo "OK: $(javac -version 2>&1 || true)"

    echo ""
    echo "== 4) Android SDK =="
    ANDROID_HOME="${ANDROID_HOME:-$HOME/android-sdk}"
    if [ ! -d "$ANDROID_HOME/cmdline-tools/latest" ]; then
        echo "Descargando Android command-line tools a $ANDROID_HOME..."
        mkdir -p "$ANDROID_HOME/cmdline-tools"
        cd /tmp/opencode
        wget -q "https://dl.google.com/android/repository/commandlinetools-linux-13114758_latest.zip" -O android-cmdline.zip
        unzip -q android-cmdline.zip
        rm -rf "$ANDROID_HOME/cmdline-tools/latest"
        mv cmdline-tools "$ANDROID_HOME/cmdline-tools/latest"

        export ANDROID_HOME="$ANDROID_HOME"
        export PATH="$ANDROID_HOME/cmdline-tools/latest/bin:$ANDROID_HOME/platform-tools:$PATH"

        yes | "$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager" --licenses >/dev/null 2>&1

        echo "Instalando platform-tools, platforms;android-34.0, build-tools;34.0.0..."
        "$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager" \
            "platform-tools" \
            "platforms;android-34.0" \
            "build-tools;34.0.0" >/dev/null

        if ! grep -q "ANDROID_HOME" "$HOME/.bashrc" 2>/dev/null; then
            echo "export ANDROID_HOME=$ANDROID_HOME" >> "$HOME/.bashrc"
            echo 'export PATH="$PATH:$ANDROID_HOME/cmdline-tools/latest/bin:$ANDROID_HOME/platform-tools"' >> "$HOME/.bashrc"
        fi
    fi
    echo "OK: ANDROID_HOME=$ANDROID_HOME"
fi

echo ""
echo "== Listo =="
echo "Recarga el shell o ejecuta:"
echo "  export PATH=\$HOME/.dotnet:\$PATH ANDROID_HOME=$ANDROID_HOME"
echo "  export PATH=\$PATH:\$ANDROID_HOME/cmdline-tools/latest/bin:\$ANDROID_HOME/platform-tools"
echo ""
echo "Luego: ./build-all.sh"
