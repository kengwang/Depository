using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;

namespace Depository.Benchmarks.DepositoryFunction;

[MemoryDiagnoser(false)]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
public class AddDependencyBenchmark
{
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
    
    [Benchmark]
    public async Task AddDependency_Pure()
    {
        var description = new DependencyDescription
        {
            DependencyType = typeof(IGuidGenerator),
            ResolvePolicy = DependencyResolvePolicy.LastWin,
            Lifetime = DependencyLifetime.Singleton
        };
        await _depository.AddDependencyAsync(description);
        var relation = new DependencyRelation
        {
            RelationType = DependencyRelationType.Once,
            ImplementType = typeof(RandomGuidGenerator),
            DefaultImplementation = null
        };
        await _depository.AddRelationAsync(description, relation);
    }

    [Benchmark]
    public async Task AddDependency_Extension()
    {
        await _depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
    }
}