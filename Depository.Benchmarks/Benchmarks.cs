using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Interfaces;

namespace Depository.Benchmarks;


[MemoryDiagnoser(false)]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
public partial class Benchmarks
{

}