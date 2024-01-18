using Autofac;
using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Models.Options;
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
[MarkdownExporter]
public partial class IoCContainersBenchmarks
{
    [Benchmark]
    public IGuidGenerator Depository_SingleToDefaultImplementBenchmark()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator>(new RandomGuidGenerator());
        return depository.Resolve<IGuidGenerator>();
    }
    
    [Benchmark]
    public IGuidGenerator Depository_Optimized_SingleToDefaultImplementBenchmark()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.Option.AutoNotifyDependencyChange = false;
        depository.Option.CheckerOption = new DepositoryCheckerOption
        {
            ImplementIsInheritedFromDependency = false,
            ImplementIsInstantiable = false,
            AutoConstructor = false,
            CheckImplementationDuplication = false
        };
        depository.Option.ImplementTypeDuplicatedAction = ImplementTypeDuplicatedAction.Continue;
        depository.AddSingleton<IGuidGenerator>(new RandomGuidGenerator());
        return depository.Resolve<IGuidGenerator>();
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