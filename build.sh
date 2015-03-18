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
  tools/nuget/NuGet.exe install FAKE -OutputDirectory packages -ExcludeVersion
  packages/FAKE/tools/FAKE.exe build.fsx $@
else
  # use mono
  echo Installing build tools...
  mono tools/nuget/NuGet.exe install FAKE -OutputDirectory packages -ExcludeVersion
  mono packages/FAKE/tools/FAKE.exe $@ --fsiargs -d:MONO build.fsx
fi