# HalKit - HAL API Client & Server Library for .NET

[![NuGet version](https://badge.fury.io/nu/halkit.svg)][nuget]
[![Downloads](https://img.shields.io/nuget/dt/halkit.svg)][nuget]
[![HalKit - CI](https://github.com/viagogo/HalKit/actions/workflows/halkit-ci.yml/badge.svg?branch=master)][ci-build]

[nuget]: http://www.nuget.org/packages/HalKit
[ci-build]: https://github.com/viagogo/HalKit/actions/workflows/halkit-ci.yml

HalKit is a lightweight library, targeting .NET 4.6.1 and above, that provides an easy way to interact with [HAL][hal] hypermedia APIs.

[hal]: http://stateless.co/hal_specification.html


## Getting Started

HalKit is available on NuGet.

```
Install-Package HalKit
```


## Usage

Create models for API resources.

```c#
public class RootResource : Resource
{
    // Use RelAttribute to (de)serialize links
    [Rel("ea:orders")]
    public Link OrdersLink { get; set; }
}

public class OrdersResource : Resource
{
    // Use the EmbeddedAttribute to (de)serialize embedded resources
    [Embedded("ea:order")]
    IList<OrderResource> Orders { get; set; }
}

public class OrderResource : Resource
{
    public string Status { get; set; }

    [Rel("ea:customer")]
    public Link CustomerLink { get; set; }
}
```

Clients can use a HalClient to access resources.

```c#
var api = new HalClient(new HalKitConfiguration("http://orders-api.com"));
var root = await api.GetRootAsync<RootResource>();

// Use the HalClient to follow hypermedia links
var ordersResource = await api.GetAsync<OrdersResource>(root.OrdersLink);
foreach (var order in ordersResource.Orders)
{
    Console.WriteLine(order.Status);
}
```


## Supported Platforms
* .NET Core 3.1
* .NET Framework 4.6.1
* Mono 5.4
* Xamarin.iOS 10.14
* Xamarin.Mac 3.8
* Xamarin.Android 7.5
* Universal Windows Platform vNext


## How to contribute

All submissions are welcome. Fork the repository, read the rest of this README
file and make some changes. Once you're done with your changes send a pull
request. Thanks!


## Need Help? Found a bug?

Just [submit a issue][submitanissue] if you need any help. And, of course, feel
free to submit pull requests with bug fixes or changes.
