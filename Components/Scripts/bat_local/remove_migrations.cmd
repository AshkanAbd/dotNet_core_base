@echo off
cd ../../../
dotnet ef database update 0
dotnet ef migrations remove 
dotnet ef migrations remove 
echo y | dotnet ef database drop 
