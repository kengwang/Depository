using Autofac;
using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models.Options;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Benchmarks;

[MemoryDiagnoser(false)]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
[MarkdownExporter]
public partial class IoCContainersBenchmarks
{
    private IDepository _depositorySingleToSingle = null!;
    private IDepository _depositoryOptimizedSingleToSingle = null!;
    private ServiceProvider _microsoftSingleToSingle = null!;
    private IContainer _autofacSingleToSingle = null!;

    private IDepository _depositorySingleToDefault = null!;
    private IDepository _depositoryOptimizedSingleToDefault = null!;
    private ServiceProvider _microsoftSingleToDefault = null!;
    private IContainer _autofacSingleToDefault = null!;

    private IDepository _depositoryMultiToSingle = null!;
    private IDepository _depositoryOptimizedMultiToSingle = null!;
    private ServiceProvider _microsoftMultiToSingle = null!;
    private IContainer _autofacMultiToSingle = null!;

    private IDepository _depositoryMultiToMulti = null!;
    private IDepository _depositoryOptimizedMultiToMulti = null!;
    private ServiceProvider _microsoftMultiToMulti = null!;
    private IContainer _autofacMultiToMulti = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _depositorySingleToSingle = DepositoryFactory.CreateNew();
        _depositorySingleToSingle.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        _depositoryOptimizedSingleToSingle = CreateOptimizedDepository();
        _depositoryOptimizedSingleToSingle.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        _microsoftSingleToSingle = CreateMicrosoftProvider(services =>
            services.AddSingleton<IGuidGenerator, RandomGuidGenerator>());
        _autofacSingleToSingle = CreateAutofacContainer(builder =>
            builder.RegisterType<RandomGuidGenerator>().As<IGuidGenerator>());

        var defaultGenerator = new RandomGuidGenerator();
        _depositorySingleToDefault = DepositoryFactory.CreateNew();
        _depositorySingleToDefault.AddSingleton<IGuidGenerator>(defaultGenerator);
        _depositoryOptimizedSingleToDefault = CreateOptimizedDepository();
        _depositoryOptimizedSingleToDefault.AddSingleton<IGuidGenerator>(defaultGenerator);
        _microsoftSingleToDefault = CreateMicrosoftProvider(services =>
            services.AddSingleton<IGuidGenerator>(defaultGenerator));
        _autofacSingleToDefault = CreateAutofacContainer(builder =>
            builder.RegisterInstance(defaultGenerator).As<IGuidGenerator>());

        _depositoryMultiToSingle = DepositoryFactory.CreateNew();
        AddMultiRegistrations(_depositoryMultiToSingle);
        _depositoryOptimizedMultiToSingle = CreateOptimizedDepository();
        AddMultiRegistrations(_depositoryOptimizedMultiToSingle);
        _microsoftMultiToSingle = CreateMicrosoftProvider(AddMicrosoftMultiRegistrations);
        _autofacMultiToSingle = CreateAutofacContainer(AddAutofacMultiRegistrations);

        _depositoryMultiToMulti = DepositoryFactory.CreateNew();
        AddMultiRegistrations(_depositoryMultiToMulti);
        _depositoryOptimizedMultiToMulti = CreateOptimizedDepository();
        AddMultiRegistrations(_depositoryOptimizedMultiToMulti);
        _microsoftMultiToMulti = CreateMicrosoftProvider(AddMicrosoftMultiRegistrations);
        _autofacMultiToMulti = CreateAutofacContainer(AddAutofacMultiRegistrations);
    }

    private static IDepository CreateOptimizedDepository()
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
        return depository;
    }

    private static void AddMultiRegistrations(IDepository depository)
    {
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
    }

    private static ServiceProvider CreateMicrosoftProvider(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        configure(services);
        return services.BuildServiceProvider();
    }

    private static void AddMicrosoftMultiRegistrations(IServiceCollection services)
    {
        services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        services.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
    }

    private static IContainer CreateAutofacContainer(Action<ContainerBuilder> configure)
    {
        var builder = new ContainerBuilder();
        configure(builder);
        return builder.Build();
    }

    private static void AddAutofacMultiRegistrations(ContainerBuilder builder)
    {
        builder.RegisterType<RandomGuidGenerator>().As<IGuidGenerator>();
        builder.RegisterType<EmptyGuidGenerator>().As<IGuidGenerator>();
    }
}
