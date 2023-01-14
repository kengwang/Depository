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
    
    //[Benchmark]
    [Obsolete("Disable Heavy Load Text")]
    public async Task<IGuidGenerator> HeavyLoad_IEnumerable()
    {
        var depository = DepositoryFactory.CreateNew();

        for (var i = 0; i < 1_000_000; i++)
        {
            await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        }

        var guidGenerators = await depository.ResolveAsync<IEnumerable<IGuidGenerator>>();
        return guidGenerators.First();
    }

    //[Benchmark]
    [Obsolete("Disable Heavy Load Text")]
    public async Task<List<IGuidGenerator>> HeavyLoad_ResolveMultiple()
    {
        // For Benchmark Purpose only
        var depository = DepositoryFactory.CreateNew(options=>options.ImplementTypeDuplicatedAction = ImplementTypeDuplicatedAction.Continue);
        for (var i = 0; i < 1_000_000; i++)
        {
            await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        }

        return await depository.ResolveMultipleAsync<IGuidGenerator>();
    }
}