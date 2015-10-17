#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open Fake.UserInputHelper
open System
open System.IO

let solutionFile  = "Regex.sln"

let testAssemblies = "**/bin/Release/*Tests.dll"

let (|Fsproj|Csproj|Vbproj|) (projFileName:string) = 
    match projFileName with
    | f when f.EndsWith("fsproj") -> Fsproj
    | f when f.EndsWith("csproj") -> Csproj
    | f when f.EndsWith("vbproj") -> Vbproj
    | _                           -> failwith (sprintf "Project file %s not supported. Unknown project type." projFileName)


Target "clean" (fun _ ->
    CleanDirs ["bin"; "temp"; 
            "Lib/bin/Release";
            "Lib/bin/Release";
            "Tests/bin/Debug";
            "Tests/bin/Debug"
            ] 
)

Target "build" (fun _ ->
    !! solutionFile
    |> MSBuildRelease "" "Rebuild"
    |> ignore
)

Target "test" (fun _ ->
    !! testAssemblies
    |> NUnit (fun p ->
        { p with
            DisableShadowCopy = true
            TimeOut = TimeSpan.FromMinutes 20.
            OutputFile = "TestResults.xml" })
)


Target "pack" (fun _ ->
    Paket.Pack(fun p -> 
        { p with
            OutputPath = "bin"})
)

Target "push" (fun _ ->
    Paket.Push(fun p -> 
        { p with
            WorkingDir = "bin" })
)

Target "all" DoNothing
"clean"
  ==> "pack"

"clean"
  ==> "build"
  ==> "test"
  ==> "all"

"pack"
  ==> "push"

RunTargetOrDefault "test"
