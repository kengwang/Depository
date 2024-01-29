# Home

Welcome to use Depository!

## Getting Started

You can easily get `Depository` from `nuget`

```shell
dotnet add package Depository
```

Then, you can create a Depository using DepositoryFactory

This will add `Depository` as `IDepository` into Depository so you can resolve it

```csharp
var depository = DepositoryFactory.CreateNew();
```

You can add then add Your first dependency

```csharp
depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
```

You don't have to build a container, because `Depository` is a container

You can then resolve `IGuidGenerator`

```csharp
var generator = depository.Resolve<IGuidGenerator>();
```

Congratulation, you successfully learned how to use it!

## Features Guide

* [Async Construct](./async_construct.md)
* [Resolve with Constructor Parameter That Are Not In Container](./customize_injectable_constructor_parameter.md)
* [Intergrate with `Microsoft.Extensions.Hosting`](./intergrate_with_meh.md)
* [Proxy/Decoration Service Support](./proxy_service.md)