@echo off
cd ../../../
echo y | dotnet ef database drop
dotnet ef database update --no-build