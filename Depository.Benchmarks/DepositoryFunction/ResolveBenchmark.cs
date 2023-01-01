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
public class ResolveBenchmark
{
    private IDepository _depository = null!;

    [IterationSetup]
    public async void IterationSetup()
    {
        _depository = DepositoryFactory.CreateNew();
        await _depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
    }

    [Benchmark]
    public async Task<IGuidGenerator> ResolveSingleToSingle()
    {
        return await _depository.ResolveAsync<IGuidGenerator>();
    }

    [Benchmark]
    public async Task<IGuidGenerator> ResolveMultipleToSingle()
    {
        await _depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        return await _depository.ResolveAsync<IGuidGenerator>();
    }

    [Benchmark]
    public async Task ResolveMultipleToMultiple_UsingIEnumerable()
    {
        await _depository.ResolveAsync<IEnumerable<IGuidGenerator>>();
    }

    [Benchmark]
    public async Task ResolveMultipleToMultiple_UsingMultipleResolve()
    {
        await _depository.ResolveMultipleAsync<IGuidGenerator>();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        _depository.Dispose();
    }
}