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
    public async Task Depository_MultiToMultiBenchmark_IEnumerable()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        await depository.ResolveAsync<IEnumerable<IGuidGenerator>>();
    }

    [Benchmark]
    public async Task Depository_MultiToMultiBenchmark_ResolveMultiple()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        await depository.ResolveMultipleAsync<IGuidGenerator>();
    }


    [Benchmark]
    public object MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark()
    {
        ServiceCollection services = new();
        services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        services.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        var provider = services.BuildServiceProvider();
        return provider.GetService<IEnumerable<IGuidGenerator>>()!;
    }

    [Benchmark]
    public object AutoFac_MultiToMultiBenchmark()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<RandomGuidGenerator>().As<IGuidGenerator>();
        builder.RegisterType<EmptyGuidGenerator>().As<IGuidGenerator>();
        var container = builder.Build();
        return container.Resolve<IEnumerable<IGuidGenerator>>();
    }
}