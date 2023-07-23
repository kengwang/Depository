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
    public void Depository_MultiToMultiBenchmark_IEnumerable()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.Resolve<IEnumerable<IGuidGenerator>>();
    }

    [Benchmark]
    public void Depository_MultiToMultiBenchmark_ResolveMultiple()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.ResolveMultiple<IGuidGenerator>();
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