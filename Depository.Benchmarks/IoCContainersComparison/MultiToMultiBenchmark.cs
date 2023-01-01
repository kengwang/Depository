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
public class MultiToMultiBenchmark
{
    [Benchmark]
    public async Task<IEnumerable<IGuidGenerator>> Depository_IEnumerable()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        return await depository.ResolveAsync<IEnumerable<IGuidGenerator>>();
    }
    
    [Benchmark]
    public async Task<IEnumerable<IGuidGenerator>> Depository_ResolveMultiple()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        return await depository.ResolveMultipleAsync<IGuidGenerator>();
    }
    

    [Benchmark]
    public IEnumerable<IGuidGenerator> MicrosoftExtensionDependencyInjection()
    {
        ServiceCollection services = new();
        services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        services.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        var provider = services.BuildServiceProvider();
        return provider.GetService<IEnumerable<IGuidGenerator>>()!;
    }
    
    [Benchmark]
    public IEnumerable<IGuidGenerator> AutoFac()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<RandomGuidGenerator>().As<IGuidGenerator>();
        builder.RegisterType<EmptyGuidGenerator>().As<IGuidGenerator>();
        var container = builder.Build();
        return container.Resolve<IEnumerable<IGuidGenerator>>();
    }
}