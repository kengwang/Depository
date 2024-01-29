# Intergrate with `Microsoft.Extensions.Hosting`

As `Microsoft.Extensions.Hosting` is being used commonly in projects (like `ASP .NET Core`), you can use Depository in those projects.

First, install package using nuget

```shell
dotnet add package Depository.Extensions.DependencyInjection
```

Then, add a line right after your builder

```csharp
host.ConfigureContainer(new DepositoryServiceProviderFactory());
```