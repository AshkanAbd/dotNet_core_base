#!/usr/bin/env bash
cd "$(dirname -- "$(realpath -- "$0")")" || exit
cd ../../../ || exit
echo 'Drop current database...'
echo yes | dotnet ef database drop --context BaseContext >>/dev/null 2>&1;
echo 'Create and migrate database...'
dotnet ef database update --context ParbadDataContext >>/dev/null 2>&1;
dotnet ef database update --context BaseContext >>/dev/null 2>&1;
echo 'Operation was successful'
exit
