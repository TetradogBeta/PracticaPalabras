#!/usr/bin/env bash
# clean.sh — Limpia el estado parcial de la instalación de Android workload
set -euo pipefail

cd "$(dirname "$0")"

ALSO_SDK=0
ALSO_JDK=0
if [ "${1:-}" = "--also-sdk" ] || [ "${1:-}" = "--all" ]; then
    ALSO_SDK=1
fi
if [ "${1:-}" = "--also-jdk" ] || [ "${1:-}" = "--all" ]; then
    ALSO_JDK=1
fi

echo "== 1) dotnet workload clean =="
if command -v dotnet >/dev/null 2>&1; then
    dotnet workload clean 2>&1 | tail -10 || echo "AVISO: clean falló, continuando"
fi

echo ""
echo "== 2) Borrando $HOME/.dotnet-temp/ =="
if [ -d "$HOME/.dotnet-temp" ]; then
    rm -rf "$HOME/.dotnet-temp"
    echo "OK"
else
    echo "no existía"
fi

echo ""
echo "== 3) Borrando paquetes Android parciales =="
DELETED=0
for pattern in \
    "Microsoft.Android.Runtime" \
    "Microsoft.Android.Sdk" \
    "Microsoft.Android.Sdk.Linux" \
    "Microsoft.Android.Ref" \
    "Microsoft.Android.Toolkit" \
    "Microsoft.Android.Arm" \
    "Microsoft.Android.Arm64" \
    "Microsoft.Android.X64"; do
    for dir in "$HOME/.dotnet/packs/${pattern}"*; do
        if [ -d "$dir" ]; then
            rm -rf "$dir"
            echo "  $dir"
            DELETED=$((DELETED+1))
        fi
    done
done

echo ""
echo "== 4) Borrando manifiestos dotnet =="
for pattern in "9.0.100" "9.0.200" "9.0.300"; do
    dir="$HOME/.dotnet/sdk-manifests/$pattern"
    if [ -d "$dir" ]; then
        rm -rf "$dir"
        echo "  $dir"
        DELETED=$((DELETED+1))
    fi
done

if [ "$DELETED" -eq 0 ]; then
    echo "  nada que borrar"
fi

if [ "$ALSO_SDK" -eq 1 ]; then
    echo ""
    echo "== 5) Borrando Android SDK ($HOME/android-sdk) =="
    if [ -d "$HOME/android-sdk" ]; then
        rm -rf "$HOME/android-sdk"
        echo "OK"
    else
        echo "no existía"
    fi
fi

if [ "$ALSO_JDK" -eq 1 ]; then
    echo ""
    echo "== 6) Desinstalando OpenJDK 17 =="
    if command -v apt-get >/dev/null 2>&1; then
        sudo apt-get purge -y openjdk-17-jdk* 2>&1 | tail -3 || true
        sudo apt-get autoremove -y 2>&1 | tail -2 || true
    elif command -v dnf >/dev/null 2>&1; then
        sudo dnf remove -y java-17-openjdk-devel 2>&1 | tail -3 || true
    elif command -v pacman >/dev/null 2>&1; then
        sudo pacman -Rns --noconfirm jdk17-openjdk 2>&1 | tail -3 || true
    fi
fi

echo ""
echo "== Estado final =="
if command -v dotnet >/dev/null 2>&1; then
    echo "Workloads:"
    dotnet workload list 2>&1 || true
fi
echo ""
echo "Espacio ocupado por .NET:"
du -sh "$HOME/.dotnet" 2>/dev/null || true

echo ""
echo "OK. Ahora puedes ejecutar:"
echo "  ./install-prereqs.sh"
echo ""
echo "Con --all borra también Android SDK y JDK:"
echo "  ./clean.sh --all"
