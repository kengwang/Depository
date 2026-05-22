using BenchmarkDotNet.Attributes;
using Depository.Benchmarks.Interfaces;
using Depository.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Benchmarks;

public partial class IoCContainersBenchmarks
{
    [Benchmark]
    public IGuidGenerator Depository_SingleToDefaultImplementBenchmark()
    {
        return _depositorySingleToDefault.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator Depository_Optimized_SingleToDefaultImplementBenchmark()
    {
        return _depositoryOptimizedSingleToDefault.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark()
    {
        return _microsoftSingleToDefault.GetRequiredService<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator AutoFac_SingleToDefaultImplementBenchmark()
    {
        return Autofac.ResolutionExtensions.Resolve<IGuidGenerator>(_autofacSingleToDefault);
    }
}
