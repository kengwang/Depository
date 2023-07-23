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
    public async Task ResolveMultipleToMultiple_UsingMultipleResolve()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.ResolveMultiple<IGuidGenerator>();
    }
}