# Async Construct

If you have to do some async work during construct, you can implement `IAsyncConstruct` interface to let Depository know.

```csharp
public class AsyncConstructorService : IAsyncConstructService
{
    // You can do some async work here
    public async Task InitializeService()
    {
        await Task.Delay(1000);
    }
}
```

Depository will invoke `InitializeService` after construct.

You can resolve `Task<T>` so that you can await the async construct.

```csharp
var service = await depository.Resolve<Task<IAsyncConstructService>>();
```



You can also resolve `IAsyncConstructService` directly, Depository will invoke `InitializeService` as a fire-and-forget.

```csharp
var service = depository.Resolve<IAsyncConstructService>();
```

> [!Warning]
>
> If you use fire-and-forget way, the async constructor will be invoked after the resolve method returns.
