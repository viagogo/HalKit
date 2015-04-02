#r @"tools/FAKE.Core/tools/FakeLib.dll"

open System
open System.IO
open Fake
open Fake.AssemblyInfoFile
open Fake.XUnit2Helper

// Project information used to generate AssemblyInfo and .nuspec
let projectName = "HalKit"
let projectDescription = "A lightweight .NET library for consuming HAL hypermedia APIs"
let authors = ["viagogo"]
let copyright = @"Copyright viagogo " + DateTime.UtcNow.ToString("yyyy");

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
            OutputDir = testResultsDir
            HtmlOutput = true}
    )
)

Target "CreatePackage" (fun _ ->
    let tags = "HAL Hypermedia API REST viagogo"
    let dependencies = [
        ("Microsoft.Net.Http", GetPackageVersion "./packages/" "Microsoft.Net.Http")
        ("Newtonsoft.Json", GetPackageVersion "./packages/" "Newtonsoft.Json")
        ("Tavis.UriTemplates", GetPackageVersion "./packages/" "Tavis.UriTemplates")
    ]

    let inline nugetFriendlyPath (path : string) = if path.StartsWith("./") then path.Remove(0, 2) else path

    let libPortableDir = "lib/portable-net45+win+wpa81+wp80+MonoAndroid10+xamarinios10+MonoTouch10/"
    let files = [
        (nugetFriendlyPath buildDir @@ "HalKit.dll", Some libPortableDir, None)
        (nugetFriendlyPath buildDir @@ "HalKit.pdb", Some libPortableDir, None)
        (nugetFriendlyPath buildDir @@ "HalKit.xml", Some libPortableDir, None)
        ("LICENSE.txt", None, None)
        ("README.md", None, None)
        ("ReleaseNotes.md", None, None)
        ("src\HalKit\**\*.cs", Some "src", None)
    ]

    NuGet (fun p ->
        {p with
            Project = projectName
            Description = projectDescription
            Copyright = copyright
            Authors = authors
            OutputPath = packagingDir
            WorkingDir = @"."
            SymbolPackage = NugetSymbolPackage.Nuspec
            Version = releaseNotes.NugetVersion
            ReleaseNotes = toLines releaseNotes.Notes
            Dependencies = dependencies
            Files = files}) "HalKit.nuspec"
)

Target "CreatePackages" DoNothing

"Clean"
    ==> "AssemblyInfo"
    ==> "BuildApp"
    ==> "UnitTests"
    ==> "CreatePackages"

"CreatePackages"
    ==> "CreatePackage"

RunTargetOrDefault "BuildApp"