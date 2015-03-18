#!/bin/bash

export TARGET="BuildApp"
if [ "$#" -ge 1 ]
then
  TARGET=$1
fi

export BUILDMODE="Release"
if [ "$#" -ge 2 ]
then
  BUILDMODE=$2
fi

if test "$OS" = "Windows_NT"
then
  # use .Net
  echo Installing build tools...
  tools/nuget/NuGet.exe install "FAKE.Core" -OutputDirectory "tools" -ExcludeVersion -Version "3.23.0"
  tools/nuget/NuGet.exe install "xunit.runner.console" -OutputDirectory "tools" -ExcludeVersion -Version "2.0.0"
  tools\nuget\NuGet.exe install "SourceLink.Fake" -OutputDirectory "tools" -ExcludeVersion -Version "0.4.2"
  tools/FAKE.Core/tools/FAKE.exe build.fsx $@
else
  # use mono
  echo Installing build tools...
  mono tools/nuget/NuGet.exe install "FAKE.Core" -OutputDirectory "tools" -ExcludeVersion -Version "3.23.0"
  mono tools/nuget/NuGet.exe install "xunit.runner.console" -OutputDirectory "tools" -ExcludeVersion -Version "2.0.0"
  mono tools\nuget\NuGet.exe install "SourceLink.Fake" -OutputDirectory "tools" -ExcludeVersion -Version "0.4.2"
  mono tools/FAKE.Core/tools/FAKE.exe $@ --fsiargs -d:MONO build.fsx
fi