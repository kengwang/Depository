using System.Reflection;
using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Interfaces;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;

// ReSharper disable once CheckNamespace
namespace Depository.Benchmarks;

public partial class Benchmarks
{
    [Params(10, 100, 1_000, 1_000_000)] public int IterationTime { get; set; }

    public async Task<IEnumerable<IGuidGenerator>> HeavyLoad_IEnumerable()
    {
        var depository = DepositoryFactory.CreateNew();

        for (var i = 0; i < IterationTime; i++)
        {
            await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        }

        return await depository.ResolveAsync<IEnumerable<IGuidGenerator>>();
    }

    public async Task<IEnumerable<IGuidGenerator>> HeavyLoad_ResolveMultiple()
    {
        
        var depository = DepositoryFactory.CreateNew();
        for (var i = 0; i < IterationTime; i++)
        {
            await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        }

        return await depository.ResolveMultipleAsync<IGuidGenerator>();
    }
}