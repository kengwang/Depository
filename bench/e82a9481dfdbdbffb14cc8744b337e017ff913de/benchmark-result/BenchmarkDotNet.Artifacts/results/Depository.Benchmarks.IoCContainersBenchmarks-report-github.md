``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 7.0.1 (7.0.122.56804), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.1 (7.0.122.56804), X64 RyuJIT AVX2


```
|                                                                  Method |       Mean |     Error |    StdDev |     Median | Allocated |
|------------------------------------------------------------------------ |-----------:|----------:|----------:|-----------:|----------:|
|                            Depository_MultiToMultiBenchmark_IEnumerable |   5.772 μs | 0.0838 μs | 0.0784 μs |   5.755 μs |   3.74 KB |
|                        Depository_MultiToMultiBenchmark_ResolveMultiple |   4.760 μs | 0.0591 μs | 0.0553 μs |   4.769 μs |   3.61 KB |
|             MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark |   4.218 μs | 0.0461 μs | 0.0431 μs |   4.206 μs |   4.51 KB |
|                                           AutoFac_MultiToMultiBenchmark | 122.995 μs | 0.7898 μs | 0.7001 μs | 122.774 μs |  24.19 KB |
|                                       Depository_MultiToSingleBenchmark |   4.020 μs | 0.0462 μs | 0.0432 μs |   4.025 μs |   3.27 KB |
|            MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark |   2.832 μs | 0.0559 μs | 0.0549 μs |   2.842 μs |   3.95 KB |
|                                          AutoFac_MultiToSingleBenchmark |  37.278 μs | 0.7443 μs | 0.8571 μs |  36.888 μs |  16.26 KB |
|                            Depository_SingleToDefaultImplementBenchmark |   2.976 μs | 0.0408 μs | 0.0382 μs |   2.979 μs |   2.55 KB |
| MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark |   3.041 μs | 0.1728 μs | 0.5096 μs |   3.088 μs |    3.6 KB |
|                               AutoFac_SingleToDefaultImplementBenchmark |  28.468 μs | 0.2443 μs | 0.2285 μs |  28.577 μs |  13.31 KB |
|                                               Depository_SingleToSingle |   3.695 μs | 0.0477 μs | 0.0446 μs |   3.680 μs |   2.91 KB |
|                    MicrosoftExtensionDependencyInjection_SingleToSingle |   3.364 μs | 0.1798 μs | 0.5301 μs |   3.566 μs |    3.8 KB |
|                                                  AutoFac_SingleToSingle |  30.065 μs | 0.1138 μs | 0.1009 μs |  30.076 μs |  13.79 KB |
