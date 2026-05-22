using BenchmarkDotNet.Attributes;
using Depository.Benchmarks.Interfaces;
using Depository.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Benchmarks;

public partial class IoCContainersBenchmarks
{
    [Benchmark]
    public IGuidGenerator Depository_MultiToSingleBenchmark()
    {
        return _depositoryMultiToSingle.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator Depository_Optimized_MultiToSingleBenchmark()
    {
        return _depositoryOptimizedMultiToSingle.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark()
    {
        return _microsoftMultiToSingle.GetRequiredService<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator AutoFac_MultiToSingleBenchmark()
    {
        return Autofac.ResolutionExtensions.Resolve<IGuidGenerator>(_autofacMultiToSingle);
    }
}
