@echo off
cd ../../../
dotnet ef migrations add Seeds 
dotnet ef database update
