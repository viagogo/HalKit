# HalKit - HAL API Client Library for .NET

[![Build status](https://ci.appveyor.com/api/projects/status/1my5ktlw22p6aynb/branch/master?svg=true)][appveyor]

[appveyor]: https://ci.appveyor.com/project/akilb/halkit/branch/master

HalKit is a lightweight client library, targeting .NET 4.5 and above, that provides an easy way to interact with HAL hypermedia APIs.

## Usage

```c#
var api = new HalClient(new HalKitConfiguration("http://hal-api.com"));
var root = await api.GetRootAsync();

// Use the HalClient to follow hypermedia links
var fooResource = await api.GetAsync<FooResource>(root.Links["foo"]);
```

## Supported Platforms

* .NET 4.5 (Desktop / Server)
* Windows 8 / 8.1 Store Apps
* Windows Phone 8 / 8.1
* Xamarin.iOS / Xamarin.Android / Xamarin.Mac
* Mono 3.x

## Getting Started

HalKit is available on NuGet.

```
Install-Package HalKit
```