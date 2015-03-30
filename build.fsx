#r @"tools/FAKE.Core/tools/FakeLib.dll"
#load "tools/SourceLink.Fake/tools/SourceLink.fsx"

open System
open System.IO
open Fake
open Fake.AssemblyInfoFile
open Fake.XUnit2Helper
open SourceLink

// Project information used to generate AssemblyInfo and .nuspec
let projectName = "HalKit"
let projectDescription = "A lightweight .NET library for consuming HAL hypermedia APIs"
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
            OutputDir = testResultsDir
            HtmlOutput = true}
    )
)

Target "SourceLink" (fun _ ->
    use repo = new GitRepo(__SOURCE_DIRECTORY__)
    let proj = VsProj.LoadRelease "src/HalKit/HalKit.csproj"
    let pdb = new PdbFile(buildDir @@ "HalKit.pdb")
    let pdbSrcSrvPath = buildDir @@ "HalKit.srcsrv"

    logfn "source linking %s" pdb.Path
    let files = (proj.Compiles -- "SolutionInfo.cs").SetBaseDirectory __SOURCE_DIRECTORY__
    repo.VerifyChecksums files
    pdb.VerifyChecksums files |> ignore

    // Make sure that we don't hold onto a file lock on the .pdb
    pdb.Dispose()

    let pdbSrcSrvBytes = SrcSrv.create "https://raw.githubusercontent.com/viagogo/halkit/{0}/%var2%" repo.Revision (repo.Paths files)
    File.WriteAllBytes(pdbSrcSrvPath, pdbSrcSrvBytes)
    Pdbstr.exec pdb.Path pdbSrcSrvPath
)

Target "CreatePackage" (fun _ ->
    CopyFiles buildDir ["LICENSE.txt"; "README.md"; "ReleaseNotes.md"]

    let tags = "HAL Hypermedia API REST"
    let dependencies = [
        ("Microsoft.Net.Http", GetPackageVersion "./packages/" "Microsoft.Net.Http")
        ("Newtonsoft.Json", GetPackageVersion "./packages/" "Newtonsoft.Json")
        ("Tavis.UriTemplates", GetPackageVersion "./packages/" "Tavis.UriTemplates")
    ]
    let libPortableDir = "lib/portable-net45+win+wpa81+wp80+MonoAndroid10+xamarinios10+MonoTouch10/"
    let files = [
        ("HalKit.dll", Some libPortableDir, None)
        ("HalKit.pdb", Some libPortableDir, None)
        ("HalKit.xml", Some libPortableDir, None)
        ("LICENSE.txt", None, None)
        ("README.md", None, None)
        ("ReleaseNotes.md", None, None)
    ]

    NuGet (fun p ->
        {p with
            Project = projectName
            Description = projectDescription
            Copyright = copyright
            Authors = authors
            OutputPath = packagingDir
            WorkingDir = buildDir
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
    // ==> "SourceLink" 
    ==> "CreatePackage"

"CreatePackage"
    ==> "CreatePackages"

RunTargetOrDefault "BuildApp"