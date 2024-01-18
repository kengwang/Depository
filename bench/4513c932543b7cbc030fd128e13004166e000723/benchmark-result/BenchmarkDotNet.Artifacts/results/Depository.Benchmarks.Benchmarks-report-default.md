
BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK=8.0.101
  [Host]     : .NET 7.0.15 (7.0.1523.57226), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.15 (7.0.1523.57226), X64 RyuJIT AVX2


                                             Method |     Mean |     Error |    StdDev | Allocated |
--------------------------------------------------- |---------:|----------:|----------:|----------:|
                                PublishNotification | 5.952 μs | 0.1108 μs | 0.1036 μs |   5.31 KB |
                      PublishNotificationWithResult | 5.998 μs | 0.0448 μs | 0.0374 μs |   5.26 KB |
                              ResolveSingleToSingle | 3.726 μs | 0.0495 μs | 0.0463 μs |   3.68 KB |
                            ResolveMultipleToSingle | 3.957 μs | 0.0658 μs | 0.0615 μs |   3.98 KB |
         ResolveMultipleToMultiple_UsingIEnumerable | 5.503 μs | 0.1052 μs | 0.0984 μs |   4.66 KB |
     ResolveMultipleToMultiple_UsingMultipleResolve | 4.872 μs | 0.0758 μs | 0.0672 μs |   4.48 KB |
                          ResolveSingleToSingle_Opt | 3.691 μs | 0.0528 μs | 0.0494 μs |    3.6 KB |
                        ResolveMultipleToSingle_Opt | 3.847 μs | 0.0502 μs | 0.0445 μs |    3.8 KB |
     ResolveMultipleToMultiple_UsingIEnumerable_Opt | 5.607 μs | 0.0751 μs | 0.0703 μs |   4.48 KB |
 ResolveMultipleToMultiple_UsingMultipleResolve_Opt | 4.398 μs | 0.0649 μs | 0.0607 μs |    4.3 KB |
