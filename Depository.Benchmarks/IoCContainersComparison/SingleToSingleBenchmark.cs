using BenchmarkDotNet.Attributes;
using Depository.Benchmarks.Interfaces;
using Depository.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Benchmarks;

public partial class IoCContainersBenchmarks
{
    [Benchmark]
    public IGuidGenerator Depository_SingleToSingle()
    {
        return _depositorySingleToSingle.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator Depository_Optimized_SingleToSingle()
    {
        return _depositoryOptimizedSingleToSingle.Resolve<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator MicrosoftExtensionDependencyInjection_SingleToSingle()
    {
        return _microsoftSingleToSingle.GetRequiredService<IGuidGenerator>();
    }

    [Benchmark]
    public IGuidGenerator AutoFac_SingleToSingle()
    {
        return Autofac.ResolutionExtensions.Resolve<IGuidGenerator>(_autofacSingleToSingle);
    }
}
