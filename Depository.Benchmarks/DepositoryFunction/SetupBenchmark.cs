using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Interfaces;
using Depository.Core;

namespace Depository.Benchmarks.DepositoryFunction;

[MemoryDiagnoser(false)]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
public class SetupBenchmark
{
    public IDepository Setup_Clean()
    {
        return new Core.Depository();
    }

    public IDepository Setup_Factory()
    {
        return DepositoryFactory.CreateNew();
    }
}