# Constructor Parameter That Are Not In Container

If you have some parameter in constructor that are not in DI Container, it is accessable outside before the resolve. You can use `FatherImplementation` in ResolveOption.

For demo, I have a Service which should pass down a string and it is absolutely not in Depository

```csharp
public class CustomFatherService : IService
{
    public CustomFatherService(string weirdParam)
    {
        
    }
}
```

Then you can resolve it using ResolveOption

```csharp
var result = depository.Resolve<IService>(fatherImplementations: new Dictionary<Type, object>
{ { typeof(string), "try use IOption" } });
```

It will invoke constructor with the value in the dictionary

> [!NOTE]
> 
> FatherImplementations has highter priority than dependencies in Depository