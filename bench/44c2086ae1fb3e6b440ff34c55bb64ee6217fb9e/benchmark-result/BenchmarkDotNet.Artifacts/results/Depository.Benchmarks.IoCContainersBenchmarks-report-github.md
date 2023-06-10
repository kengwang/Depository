``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
|                                                                  Method |       Mean |     Error |    StdDev | Allocated |
|------------------------------------------------------------------------ |-----------:|----------:|----------:|----------:|
|                            Depository_MultiToMultiBenchmark_IEnumerable |   4.120 μs | 0.0273 μs | 0.0256 μs |   3.91 KB |
|                        Depository_MultiToMultiBenchmark_ResolveMultiple |   3.380 μs | 0.0675 μs | 0.0693 μs |   3.77 KB |
|             MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark |   3.984 μs | 0.0750 μs | 0.0702 μs |   4.51 KB |
|                                           AutoFac_MultiToMultiBenchmark | 121.285 μs | 0.5048 μs | 0.4475 μs |   24.2 KB |
|                                       Depository_MultiToSingleBenchmark |   2.404 μs | 0.0222 μs | 0.0208 μs |   3.13 KB |
|            MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark |   2.773 μs | 0.0542 μs | 0.0532 μs |   3.95 KB |
|                                          AutoFac_MultiToSingleBenchmark |  35.194 μs | 0.2416 μs | 0.2260 μs |  16.26 KB |
|                            Depository_SingleToDefaultImplementBenchmark |   1.333 μs | 0.0202 μs | 0.0179 μs |   2.23 KB |
| MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark |   2.914 μs | 0.1731 μs | 0.5105 μs |    3.6 KB |
|                               AutoFac_SingleToDefaultImplementBenchmark |  27.691 μs | 0.1496 μs | 0.1399 μs |  13.31 KB |
|                                               Depository_SingleToSingle |   2.055 μs | 0.0215 μs | 0.0201 μs |   2.76 KB |
|                    MicrosoftExtensionDependencyInjection_SingleToSingle |   3.261 μs | 0.1859 μs | 0.5482 μs |    3.8 KB |
|                                                  AutoFac_SingleToSingle |  29.363 μs | 0.2044 μs | 0.1812 μs |  13.79 KB |
