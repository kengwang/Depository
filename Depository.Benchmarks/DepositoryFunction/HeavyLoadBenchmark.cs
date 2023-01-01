using System.Reflection;
using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Interfaces;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;

namespace Depository.Benchmarks.DepositoryFunction;

[MemoryDiagnoser(false)]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
public class HeavyLoadBenchmark
{
    [Params(10, 100, 1_000, 1_000_000)] public int IterationTime { get; set; }

    private IDepository _depository = null!;

    [IterationCleanup]
    public void GlobalSetup()
    {
        _depository = DepositoryFactory.CreateNew();
    }

    [IterationCleanup]
    public void GlobalCleanup()
    {
        _depository.Dispose();
    }

    public async Task<IEnumerable<IGuidGenerator>> HeavyLoad_IEnumerable()
    {
        for (var i = 0; i < IterationTime; i++)
        {
            await _depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        }

        return await _depository.ResolveAsync<IEnumerable<IGuidGenerator>>();
    }
    
    public async Task<IEnumerable<IGuidGenerator>> HeavyLoad_ResolveMultiple()
    {
        for (var i = 0; i < IterationTime; i++)
        {
            await _depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        }

        return await _depository.ResolveMultipleAsync<IGuidGenerator>();
    }
}