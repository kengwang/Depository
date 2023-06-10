``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
|                                                                  Method |       Mean |     Error |    StdDev | Allocated |
|------------------------------------------------------------------------ |-----------:|----------:|----------:|----------:|
|                            Depository_MultiToMultiBenchmark_IEnumerable |   7.858 μs | 0.1280 μs | 0.1134 μs |   4.57 KB |
|                        Depository_MultiToMultiBenchmark_ResolveMultiple |   6.841 μs | 0.1115 μs | 0.1043 μs |   4.44 KB |
|             MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark |   4.570 μs | 0.0633 μs | 0.0561 μs |   4.51 KB |
|                                           AutoFac_MultiToMultiBenchmark | 128.982 μs | 1.3849 μs | 1.2954 μs |   24.2 KB |
|                                       Depository_MultiToSingleBenchmark |   5.822 μs | 0.0826 μs | 0.0733 μs |   3.79 KB |
|            MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark |   4.275 μs | 0.2566 μs | 0.7565 μs |   3.95 KB |
|                                          AutoFac_MultiToSingleBenchmark |  39.492 μs | 0.6444 μs | 0.6028 μs |  16.26 KB |
|                            Depository_SingleToDefaultImplementBenchmark |   4.408 μs | 0.0578 μs | 0.0482 μs |   2.89 KB |
| MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark |   3.582 μs | 0.2313 μs | 0.6820 μs |    3.6 KB |
|                               AutoFac_SingleToDefaultImplementBenchmark |  29.024 μs | 0.3583 μs | 0.3352 μs |  13.31 KB |
|                                               Depository_SingleToSingle |   5.378 μs | 0.0959 μs | 0.1142 μs |   3.42 KB |
|                    MicrosoftExtensionDependencyInjection_SingleToSingle |   4.174 μs | 0.2737 μs | 0.8070 μs |    3.8 KB |
|                                                  AutoFac_SingleToSingle |  31.959 μs | 0.5257 μs | 0.4918 μs |  13.79 KB |
