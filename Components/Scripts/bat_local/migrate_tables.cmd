@echo off
cd ../../../
dotnet ef migrations add Tables 
dotnet ef database update
