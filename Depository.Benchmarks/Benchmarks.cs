using BenchmarkDotNet.Attributes;

namespace Depository.Benchmarks;


[MemoryDiagnoser(false)]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
public partial class Benchmarks
{

}