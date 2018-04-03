# Thompson_Regex [![Build Status](https://travis-ci.org/wallymathieu/Thompson_Regex.svg)](https://travis-ci.org/wallymathieu/Thompson_Regex) [![Build status](https://ci.appveyor.com/api/projects/status/xny5sdoebkrbmmr5/branch/master?svg=true)](https://ci.appveyor.com/project/wallymathieu/thompson-regex/branch/master)

port of https://swtch.com/~rsc/regexp/regexp1.html to f#

## Why

In order to have a regex matcher in f#. This algorithm can be reused for other usages.


## Builds

MacOS/Linux | Windows
--- | ---
[![Travis Badge](https://travis-ci.org/wallymathieu/Thompson_Regex.svg?branch=master)](https://travis-ci.org/wallymathieu/Thompson_Regex) | [![Build status](https://ci.appveyor.com/api/projects/status/github/wallymathieu/Thompson_Regex?svg=true)](https://ci.appveyor.com/project/wallymathieu/thompson-regex)
[![Build History](https://buildstats.info/travisci/chart/wallymathieu/thompson-regex)](https://travis-ci.org/wallymathieu/thompson-regex/builds) | [![Build History](https://buildstats.info/appveyor/chart/wallymathieu/thompson-regex)](https://ci.appveyor.com/project/wallymathieu/thompson-regex)  

### Building


Make sure the following **requirements** are installed in your system:

* [dotnet SDK](https://www.microsoft.com/net/download/core) 2.0 or higher
* [Mono](http://www.mono-project.com/) if you're on Linux or macOS.

```
> build.cmd // on windows
$ ./build.sh  // on unix
```

#### Environment Variables

* `CONFIGURATION` will set the [configuration](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build?tabs=netcore2x#options) of the dotnet commands.  If not set it will default to Release.
  * `CONFIGURATION=Debug ./build.sh` will result in things like `dotnet build -c Debug`
* `GITHUB_TOKEN` will be used to upload release notes and nuget packages to github.
  * Be sure to set this before releasing

### Watch Tests

The `WatchTests` target will use [dotnet-watch](https://github.com/aspnet/Docs/blob/master/aspnetcore/tutorials/dotnet-watch.md) to watch for changes in your lib or tests and re-run your tests on all `TargetFrameworks`

```
./build.sh WatchTests
```
