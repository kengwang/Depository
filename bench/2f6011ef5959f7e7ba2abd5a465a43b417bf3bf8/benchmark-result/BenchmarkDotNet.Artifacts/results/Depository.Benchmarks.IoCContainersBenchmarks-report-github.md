``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
|                                                                  Method |       Mean |     Error |    StdDev | Allocated |
|------------------------------------------------------------------------ |-----------:|----------:|----------:|----------:|
|                            Depository_MultiToMultiBenchmark_IEnumerable |   5.602 μs | 0.1119 μs | 0.2209 μs |   3.91 KB |
|                        Depository_MultiToMultiBenchmark_ResolveMultiple |   4.561 μs | 0.0911 μs | 0.1666 μs |   3.77 KB |
|             MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark |   5.863 μs | 0.1151 μs | 0.2016 μs |   4.51 KB |
|                                           AutoFac_MultiToMultiBenchmark | 198.932 μs | 3.9435 μs | 5.1277 μs |  24.19 KB |
|                                       Depository_MultiToSingleBenchmark |   3.389 μs | 0.0665 μs | 0.0841 μs |   3.13 KB |
|            MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark |   4.317 μs | 0.0853 μs | 0.2093 μs |   3.95 KB |
|                                          AutoFac_MultiToSingleBenchmark |  52.464 μs | 1.0309 μs | 1.8325 μs |  16.26 KB |
|                            Depository_SingleToDefaultImplementBenchmark |   1.890 μs | 0.0371 μs | 0.0600 μs |   2.23 KB |
| MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark |   4.245 μs | 0.2140 μs | 0.6310 μs |    3.6 KB |
|                               AutoFac_SingleToDefaultImplementBenchmark |  40.530 μs | 0.7746 μs | 1.0340 μs |  13.31 KB |
|                                               Depository_SingleToSingle |   2.997 μs | 0.0593 μs | 0.0729 μs |   2.76 KB |
|                    MicrosoftExtensionDependencyInjection_SingleToSingle |   4.804 μs | 0.2206 μs | 0.6503 μs |    3.8 KB |
|                                                  AutoFac_SingleToSingle |  41.370 μs | 0.8251 μs | 1.4233 μs |  13.79 KB |
