using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Interfaces;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;

namespace Depository.Benchmarks.DepositoryFunction;

public class ResolveBenchmark
{
    
    private IDepository _depository = null!;
    
    [IterationCleanup]
    public async void GlobalSetup()
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
    public async Task<IEnumerable<IGuidGenerator>> ResolveMultipleToMultiple_UsingIEnumerable()
    {
        return await _depository.ResolveAsync<IEnumerable<IGuidGenerator>>();
    }
    
    [Benchmark]
    public async Task<IEnumerable<IGuidGenerator>> ResolveMultipleToMultiple_UsingMultipleResolve()
    {
        return await _depository.ResolveMultipleAsync<IGuidGenerator>();
    }
    
    [IterationCleanup]
    public void GlobalCleanup()
    {
        _depository.Dispose();
    }
}