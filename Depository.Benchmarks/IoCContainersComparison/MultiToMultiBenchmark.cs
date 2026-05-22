using BenchmarkDotNet.Attributes;
using Depository.Benchmarks.Interfaces;
using Depository.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Benchmarks;

public partial class IoCContainersBenchmarks
{
    [Benchmark]
    public IGuidGenerator[] Depository_MultiToMultiBenchmark_IEnumerable()
    {
        return (IGuidGenerator[])_depositoryMultiToMulti.Resolve<IEnumerable<IGuidGenerator>>();
    }

    [Benchmark]
    public IGuidGenerator[] Depository_Optimized_MultiToMultiBenchmark_IEnumerable()
    {
        return (IGuidGenerator[])_depositoryOptimizedMultiToMulti.Resolve<IEnumerable<IGuidGenerator>>();
    }

    [Benchmark]
    public List<IGuidGenerator> Depository_Optimized_MultiToMultiBenchmark_ResolveMultiple()
    {
        return _depositoryOptimizedMultiToMulti.ResolveMultiple<IGuidGenerator>();
    }

    [Benchmark]
    public List<IGuidGenerator> Depository_MultiToMultiBenchmark_ResolveMultiple()
    {
        return _depositoryMultiToMulti.ResolveMultiple<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator[] MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark()
    {
        return _microsoftMultiToMulti.GetServices<IGuidGenerator>().ToArray();
    }

    [Benchmark]
    public IGuidGenerator[] AutoFac_MultiToMultiBenchmark()
    {
        return Autofac.ResolutionExtensions.Resolve<IEnumerable<IGuidGenerator>>(_autofacMultiToMulti).ToArray();
    }
}
