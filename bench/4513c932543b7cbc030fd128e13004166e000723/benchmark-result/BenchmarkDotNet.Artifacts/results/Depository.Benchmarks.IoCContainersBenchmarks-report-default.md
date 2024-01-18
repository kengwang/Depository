
BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK=8.0.101
  [Host]     : .NET 7.0.15 (7.0.1523.57226), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.15 (7.0.1523.57226), X64 RyuJIT AVX2


                                                                  Method |       Mean |     Error |    StdDev | Allocated |
------------------------------------------------------------------------ |-----------:|----------:|----------:|----------:|
                            Depository_MultiToMultiBenchmark_IEnumerable |   5.507 μs | 0.0648 μs | 0.0606 μs |   4.66 KB |
                  Depository_Optimized_MultiToMultiBenchmark_IEnumerable |   5.404 μs | 0.0731 μs | 0.0683 μs |   4.48 KB |
              Depository_Optimized_MultiToMultiBenchmark_ResolveMultuple |   4.350 μs | 0.0305 μs | 0.0286 μs |    4.3 KB |
                        Depository_MultiToMultiBenchmark_ResolveMultiple |   4.742 μs | 0.0604 μs | 0.0565 μs |   4.48 KB |
             MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark |   3.700 μs | 0.0296 μs | 0.0247 μs |   4.79 KB |
                                           AutoFac_MultiToMultiBenchmark | 118.939 μs | 1.9567 μs | 1.8303 μs |  24.38 KB |
                                       Depository_MultiToSingleBenchmark |   3.942 μs | 0.0606 μs | 0.0566 μs |   3.98 KB |
                             Depository_Optimized_MultiToSingleBenchmark |   3.866 μs | 0.0654 μs | 0.0727 μs |    3.8 KB |
            MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark |   2.444 μs | 0.0389 μs | 0.0345 μs |   4.23 KB |
                                          AutoFac_MultiToSingleBenchmark |  30.326 μs | 0.2945 μs | 0.2755 μs |  16.47 KB |
                            Depository_SingleToDefaultImplementBenchmark |   3.103 μs | 0.0603 μs | 0.0645 μs |   3.29 KB |
                  Depository_Optimized_SingleToDefaultImplementBenchmark |   3.141 μs | 0.0430 μs | 0.0403 μs |   3.21 KB |
 MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark |   2.039 μs | 0.0329 μs | 0.0275 μs |   3.88 KB |
                               AutoFac_SingleToDefaultImplementBenchmark |  23.225 μs | 0.1571 μs | 0.1470 μs |  13.52 KB |
                                               Depository_SingleToSingle |   3.668 μs | 0.0560 μs | 0.0496 μs |   3.68 KB |
                                     Depository_Optimized_SingleToSingle |   3.818 μs | 0.0546 μs | 0.0511 μs |    3.6 KB |
                    MicrosoftExtensionDependencyInjection_SingleToSingle |   2.374 μs | 0.0276 μs | 0.0258 μs |   4.09 KB |
                                                  AutoFac_SingleToSingle |  24.706 μs | 0.1223 μs | 0.1144 μs |     14 KB |
