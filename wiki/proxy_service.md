# Proxy Service

As the `Law of Demeter`, the other service will not know if it is using directly a service or its proxy, you can set proxy just on Depository.

```csharp
depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
depository.SetDependencyDecoration<IGuidGenerator, GuidDecorationService>();
```

and then any other place to resolve IGuidGenerator will only get `GuidDecorationService`, but in `GuidDecorationService`, you can resolve actual services.

This also applies to IEnumerable<T>