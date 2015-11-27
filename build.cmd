@echo off

echo Installing build tools...
".\tools\nuget\NuGet.exe" "Install" "FAKE.Core" "-OutputDirectory" "tools" "-ExcludeVersion" "-version" "4.9.3"
".\tools\nuget\NuGet.exe" "Install" "xunit.runner.console" "-OutputDirectory" "tools" "-ExcludeVersion" "-version" "2.1.0"

set TARGET="BuildApp"
if not [%1]==[] (set TARGET="%1")

set BUILDMODE="Release"
if not [%2]==[] (set BUILDMODE="%2")

"tools\FAKE.Core\tools\Fake.exe" build.fsx "target=%TARGET%" "buildMode=%BUILDMODE%"

:Quit
exit /b %errorlevel%