﻿using Autofac;
using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Models.Options;
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
    public IGuidGenerator Depository_SingleToSingle()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        return depository.Resolve<IGuidGenerator>();
    }
    
    [Benchmark]
    public IGuidGenerator Depository_Optimized_SingleToSingle()
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
    public IGuidGenerator MicrosoftExtensionDependencyInjection_SingleToSingle()
    {
        ServiceCollection services = new();
        services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        var provider = services.BuildServiceProvider();
        return provider.GetService<IGuidGenerator>()!;
    }

    [Benchmark]
    public IGuidGenerator AutoFac_SingleToSingle()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<RandomGuidGenerator>().As<IGuidGenerator>();
        var container = builder.Build();
        return container.Resolve<IGuidGenerator>();
    }
}