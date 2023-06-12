``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
|                                                                  Method |       Mean |     Error |    StdDev | Allocated |
|------------------------------------------------------------------------ |-----------:|----------:|----------:|----------:|
|                            Depository_MultiToMultiBenchmark_IEnumerable |   7.953 μs | 0.1546 μs | 0.2217 μs |   4.57 KB |
|                        Depository_MultiToMultiBenchmark_ResolveMultiple |   7.042 μs | 0.1404 μs | 0.2268 μs |   4.44 KB |
|             MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark |   5.026 μs | 0.1004 μs | 0.2287 μs |   4.51 KB |
|                                           AutoFac_MultiToMultiBenchmark | 172.198 μs | 3.3887 μs | 4.7505 μs |  24.19 KB |
|                                       Depository_MultiToSingleBenchmark |   6.430 μs | 0.1260 μs | 0.1807 μs |   3.79 KB |
|            MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark |   4.373 μs | 0.1533 μs | 0.4495 μs |   3.95 KB |
|                                          AutoFac_MultiToSingleBenchmark |  43.100 μs | 0.8528 μs | 1.1673 μs |  16.26 KB |
|                            Depository_SingleToDefaultImplementBenchmark |   4.477 μs | 0.0834 μs | 0.1248 μs |   2.89 KB |
| MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark |   3.492 μs | 0.2114 μs | 0.6232 μs |    3.6 KB |
|                               AutoFac_SingleToDefaultImplementBenchmark |  32.209 μs | 0.6308 μs | 0.7264 μs |  13.31 KB |
|                                               Depository_SingleToSingle |   5.386 μs | 0.1058 μs | 0.1259 μs |   3.42 KB |
|                    MicrosoftExtensionDependencyInjection_SingleToSingle |   4.097 μs | 0.2344 μs | 0.6910 μs |    3.8 KB |
|                                                  AutoFac_SingleToSingle |  35.907 μs | 0.7163 μs | 1.2545 μs |  13.79 KB |
