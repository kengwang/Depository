using BenchmarkDotNet.Attributes;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;

// ReSharper disable once CheckNamespace
namespace Depository.Benchmarks;

public partial class Benchmarks
{

    [Benchmark]
    public async Task<IGuidGenerator> ResolveSingleToSingle()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        return await depository.ResolveAsync<IGuidGenerator>();
    }

    [Benchmark]
    public async Task<IGuidGenerator> ResolveMultipleToSingle()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        return await depository.ResolveAsync<IGuidGenerator>();
    }

    [Benchmark]
    public async Task ResolveMultipleToMultiple_UsingIEnumerable()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        await depository.ResolveAsync<IEnumerable<IGuidGenerator>>();
    }

    [Benchmark]
    public async Task ResolveMultipleToMultiple_UsingMultipleResolve()
    {
        var depository = DepositoryFactory.CreateNew();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        await depository.ResolveMultipleAsync<IGuidGenerator>();
    }
}