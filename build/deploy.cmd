@echo off
setlocal EnableDelayedExpansion

set MODNAME=%1
set BUILDDIR=%2
set PROJECTDIR=%3

set DLL=%BUILDDIR%%MODNAME%.dll
set MODDIR=%PROJECTDIR%..\..\SPT\SPT\user\mods\%MODNAME%

echo ===============================
echo Deploying %MODNAME%
echo ===============================

if not exist "%MODDIR%" mkdir "%MODDIR%"

echo Cleaning old files...
for %%F in ("%MODDIR%\*") do del /q "%%F" 2>nul
for /d %%D in ("%MODDIR%\*") do rmdir /s /q "%%D" 2>nul

echo Copying DLL...
copy /Y "%DLL%" "%MODDIR%\" >nul

if exist "%PROJECTDIR%db" (
    xcopy "%PROJECTDIR%db" "%MODDIR%\db" /E /I /Y >nul
)

:: Create temporary exclude file for .cs files
set EXCLUDEFILE=%TEMP%\exclude_cs.txt
echo .cs > "%EXCLUDEFILE%"

if exist "%PROJECTDIR%config" (
    xcopy "%PROJECTDIR%config" "%MODDIR%\config" /E /I /Y /EXCLUDE:%EXCLUDEFILE% >nul
)

if exist "%PROJECTDIR%Resources" (
    xcopy "%PROJECTDIR%Resources" "%MODDIR%" /E /I /Y >nul
)

if exist "%PROJECTDIR%bundles" (
    xcopy "%PROJECTDIR%bundles" "%MODDIR%\bundles" /E /I /Y >nul
)

if exist "%PROJECTDIR%bundles.json" (
    copy /Y "%PROJECTDIR%bundles.json" "%MODDIR%" >nul
)

echo Deploy Complete.
endlocal