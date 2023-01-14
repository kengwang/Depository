``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.102
  [Host]     : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2


```
|                                                                  Method |       Mean |     Error |    StdDev | Allocated |
|------------------------------------------------------------------------ |-----------:|----------:|----------:|----------:|
|                            Depository_MultiToMultiBenchmark_IEnumerable |   5.838 μs | 0.0350 μs | 0.0310 μs |   3.94 KB |
|                        Depository_MultiToMultiBenchmark_ResolveMultiple |   4.853 μs | 0.0668 μs | 0.0592 μs |    3.8 KB |
|             MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark |   4.238 μs | 0.0645 μs | 0.0603 μs |   4.51 KB |
|                                           AutoFac_MultiToMultiBenchmark | 120.646 μs | 0.8444 μs | 0.7486 μs |   24.2 KB |
|                                       Depository_MultiToSingleBenchmark |   4.050 μs | 0.0639 μs | 0.0597 μs |   3.33 KB |
|            MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark |   3.636 μs | 0.1760 μs | 0.5190 μs |   3.95 KB |
|                                          AutoFac_MultiToSingleBenchmark |  35.984 μs | 0.2134 μs | 0.1996 μs |  16.26 KB |
|                            Depository_SingleToDefaultImplementBenchmark |   2.861 μs | 0.0240 μs | 0.0225 μs |    2.6 KB |
| MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark |   3.050 μs | 0.1792 μs | 0.5283 μs |    3.6 KB |
|                               AutoFac_SingleToDefaultImplementBenchmark |  27.496 μs | 0.1251 μs | 0.1109 μs |  13.31 KB |
|                                               Depository_SingleToSingle |   3.644 μs | 0.0367 μs | 0.0325 μs |   2.96 KB |
|                    MicrosoftExtensionDependencyInjection_SingleToSingle |   3.315 μs | 0.1840 μs | 0.5426 μs |    3.8 KB |
|                                                  AutoFac_SingleToSingle |  29.042 μs | 0.1660 μs | 0.1553 μs |  13.79 KB |
