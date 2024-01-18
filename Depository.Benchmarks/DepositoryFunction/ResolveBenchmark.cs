using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Models.Options;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;

// ReSharper disable once CheckNamespace
namespace Depository.Benchmarks;

public partial class Benchmarks
{

    [Benchmark]
    public IGuidGenerator ResolveSingleToSingle()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        return depository.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator ResolveMultipleToSingle()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        return depository.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public void ResolveMultipleToMultiple_UsingIEnumerable()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.Resolve<IEnumerable<IGuidGenerator>>();
    }

    [Benchmark]
    public void ResolveMultipleToMultiple_UsingMultipleResolve()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.ResolveMultiple<IGuidGenerator>();
    }
    
    
    // Optimized Version
    
    [Benchmark]
    public IGuidGenerator ResolveSingleToSingle_Opt()
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
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        return depository.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator ResolveMultipleToSingle_Opt()
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
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        return depository.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public void ResolveMultipleToMultiple_UsingIEnumerable_Opt()
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
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.Resolve<IEnumerable<IGuidGenerator>>();
    }

    [Benchmark]
    public async Task ResolveMultipleToMultiple_UsingMultipleResolve_Opt()
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
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.ResolveMultiple<IGuidGenerator>();
    }
}