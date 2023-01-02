using Autofac;
using BenchmarkDotNet.Attributes;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Depository.Benchmarks;

public partial class IoCContainersBenchmarks
{
    [Benchmark]
    public async Task<IGuidGenerator> Depository_MultiToSingleBenchmark()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        return await depository.ResolveAsync<IGuidGenerator>();
    }


    [Benchmark]
    public IGuidGenerator MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark()
    {
        ServiceCollection services = new();
        services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        services.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        var provider = services.BuildServiceProvider();
        return provider.GetService<IGuidGenerator>()!;
    }

    [Benchmark]
    public IGuidGenerator AutoFac_MultiToSingleBenchmark()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<RandomGuidGenerator>().As<IGuidGenerator>();
        builder.RegisterType<EmptyGuidGenerator>().As<IGuidGenerator>();
        var container = builder.Build();
        return container.Resolve<IGuidGenerator>();
    }
}