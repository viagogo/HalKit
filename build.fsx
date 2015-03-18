#r @"tools/FAKE.Core/tools/FakeLib.dll"

open System
open Fake
open Fake.AssemblyInfoFile
open Fake.XUnit2Helper

// Project information used to generate AssemblyInfo and .nuspec
let projectName = "HalKit"
let projectDescription = "A lightweight async HAL client library for .NET"
let authors = ["viagogo"]
let copyright = @"Copyright © viagogo " + DateTime.UtcNow.ToString("yyyy");

// Directories
let buildDir = @"./artifacts/"
let packagingDir = buildDir @@ "packages"
let testResultsDir = @"./testresults/"

// Read Release Notes and version from ReleaseNotes.md
let releaseNotes = 
    ReadFile "ReleaseNotes.md"
    |> ReleaseNotesHelper.parseReleaseNotes


// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testResultsDir; packagingDir]
)

Target "AssemblyInfo" (fun _ ->
    CreateCSharpAssemblyInfo "./SolutionInfo.cs"
      [ Attribute.Product projectName
        Attribute.Company authors.[0]
        Attribute.Copyright copyright
        Attribute.Version releaseNotes.AssemblyVersion
        Attribute.FileVersion releaseNotes.AssemblyVersion
        Attribute.InformationalVersion releaseNotes.NugetVersion
        Attribute.ComVisible false ]
)

Target "BuildApp" (fun _ ->
    let buildMode = getBuildParamOrDefault "buildMode" "Release"

    RestorePackages()

    MSBuild buildDir "Build" ["Configuration", buildMode] ["./HalKit.sln"]
    |> Log "AppBuild-Output: "
)

Target "UnitTests" (fun _ ->
    !! (buildDir + @"\HalKit*.Tests.dll")
    |> xUnit2 (fun p ->
        {p with
            OutputDir = testResultsDir}
    )
)

"Clean"
    ==> "AssemblyInfo"
    ==> "BuildApp"
    ==> "UnitTests"

RunTargetOrDefault "BuildApp"