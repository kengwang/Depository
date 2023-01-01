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
public class SingleToSingleBenchmark
{
    
    [Benchmark]
    public async Task<IGuidGenerator> Depository()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        return await depository.ResolveAsync<IGuidGenerator>();
    }
    

    [Benchmark]
    public IGuidGenerator MicrosoftExtensionDependencyInjection()
    {
        ServiceCollection services = new();
        services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        var provider = services.BuildServiceProvider();
        return provider.GetService<IGuidGenerator>()!;
    }
    
    [Benchmark]
    public IGuidGenerator AutoFac()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<RandomGuidGenerator>().As<IGuidGenerator>();
        var container = builder.Build();
        return container.Resolve<IGuidGenerator>();
    }
}