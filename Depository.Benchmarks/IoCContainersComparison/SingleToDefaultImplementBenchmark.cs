using Autofac;
using BenchmarkDotNet.Attributes;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Depository.Benchmarks;


[MemoryDiagnoser(false)]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
public partial class IoCContainersBenchmarks
{
    [Benchmark]
    public async Task<IGuidGenerator> Depository_SingleToDefaultImplementBenchmark()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator>(new RandomGuidGenerator());
        return await depository.ResolveAsync<IGuidGenerator>();
    }


    [Benchmark]
    public IGuidGenerator MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark()
    {
        ServiceCollection services = new();
        services.AddSingleton<IGuidGenerator>(new RandomGuidGenerator());
        var provider = services.BuildServiceProvider();
        return provider.GetService<IGuidGenerator>()!;
    }

    [Benchmark]
    public IGuidGenerator AutoFac_SingleToDefaultImplementBenchmark()
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(new RandomGuidGenerator()).As<IGuidGenerator>();
        var container = builder.Build();
        return container.Resolve<IGuidGenerator>();
    }
}