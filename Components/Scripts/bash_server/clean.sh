#!/usr/bin/env bash
cd "$(dirname -- "$(realpath -- "$0")")" || exit
cd ../../../ || exit
echo 'Clean project...'
dotnet clean --configuration Release >>/dev/null 2>&1;
echo 'Operation was successful'
