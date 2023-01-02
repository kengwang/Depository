﻿using BenchmarkDotNet.Attributes;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;

// ReSharper disable once CheckNamespace
namespace Depository.Benchmarks;

public partial class Benchmarks
{
    
    [Arguments(1_000,1_000_000)]
    [Benchmark]
    public async Task<IEnumerable<IGuidGenerator>> HeavyLoad_IEnumerable(int iterationTime)
    {
        var depository = DepositoryFactory.CreateNew();

        for (var i = 0; i < iterationTime; i++)
        {
            await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        }

        return await depository.ResolveAsync<IEnumerable<IGuidGenerator>>();
    }

    [Arguments(1_000,1_000_000)]
    [Benchmark]
    public async Task<IEnumerable<IGuidGenerator>> HeavyLoad_ResolveMultiple(int iterationTime)
    {
        
        var depository = DepositoryFactory.CreateNew();
        for (var i = 0; i < iterationTime; i++)
        {
            await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        }

        return await depository.ResolveMultipleAsync<IGuidGenerator>();
    }
}