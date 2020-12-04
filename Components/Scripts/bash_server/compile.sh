#!/usr/bin/env bash
cd "$(dirname -- "$(realpath -- "$0")")" || exit
cd ../../../ || exit
echo 'Build project...'
dotnet build --configuration Release >>/dev/null 2>&1;
echo 'Publish project...'
dotnet publish --configuration Release >>/dev/null 2>&1;
echo 'Operation was successful'
