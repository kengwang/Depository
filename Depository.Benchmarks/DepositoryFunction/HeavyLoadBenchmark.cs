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
    [Obsolete("Disable Heavy Load Test")]
    public IGuidGenerator HeavyLoad_IEnumerable()
    {
        var depository = DepositoryFactory.CreateNew();

        for (var i = 0; i < 1_000_000; i++)
        {
            depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        }

        var guidGenerators = depository.Resolve<IEnumerable<IGuidGenerator>>();
        return guidGenerators.First();
    }

    [Benchmark]
    [Obsolete("Disable Heavy Load Test")]
    public List<IGuidGenerator> HeavyLoad_ResolveMultiple()
    {
        // For Benchmark Purpose only
        var depository = DepositoryFactory.CreateNew(options=>options.ImplementTypeDuplicatedAction = ImplementTypeDuplicatedAction.Continue);
        for (var i = 0; i < 1_000_000; i++)
        {
            depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        }

        return depository.ResolveMultiple<IGuidGenerator>();
    }
}