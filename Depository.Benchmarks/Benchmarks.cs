using BenchmarkDotNet.Attributes;

namespace Depository.Benchmarks;


[MemoryDiagnoser(false)]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
[MarkdownExporter]
public partial class Benchmarks
{

}