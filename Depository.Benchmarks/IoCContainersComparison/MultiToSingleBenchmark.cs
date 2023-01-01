using Autofac;
using BenchmarkDotNet.Attributes;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Benchmarks.IoCContainersComparison;

[MemoryDiagnoser(false)]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
public class MultiToSingleBenchmark
{
    [Benchmark]
    public async Task<IGuidGenerator> Depository()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        return await depository.ResolveAsync<IGuidGenerator>();
    }
    

    [Benchmark]
    public IGuidGenerator MicrosoftExtensionDependencyInjection()
    {
        ServiceCollection services = new();
        services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        services.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        var provider = services.BuildServiceProvider();
        return provider.GetService<IGuidGenerator>()!;
    }
    
    [Benchmark]
    public IGuidGenerator AutoFac()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<RandomGuidGenerator>().As<IGuidGenerator>();
        builder.RegisterType<EmptyGuidGenerator>().As<IGuidGenerator>();
        var container = builder.Build();
        return container.Resolve<IGuidGenerator>();
    }
}