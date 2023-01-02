``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 7.0.1 (7.0.122.56804), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.1 (7.0.122.56804), X64 RyuJIT AVX2


```
|                                         Method |           Mean |         Error |        StdDev |    Allocated |
|----------------------------------------------- |---------------:|--------------:|--------------:|-------------:|
|                          HeavyLoad_IEnumerable | 302,602.093 μs |   705.9140 μs |   660.3124 μs |  359378.9 KB |
|                      HeavyLoad_ResolveMultiple | 734,744.724 μs | 9,787.3868 μs | 9,155.1277 μs | 509335.58 KB |
|                            PublishNotification |       3.735 μs |     0.0344 μs |     0.0287 μs |      2.98 KB |
|                  PublishNotificationWithResult |       4.010 μs |     0.0582 μs |     0.0544 μs |      3.23 KB |
|                          ResolveSingleToSingle |       3.717 μs |     0.0345 μs |     0.0322 μs |      2.91 KB |
|                        ResolveMultipleToSingle |       4.089 μs |     0.0473 μs |     0.0442 μs |      3.27 KB |
|     ResolveMultipleToMultiple_UsingIEnumerable |       5.783 μs |     0.0750 μs |     0.0665 μs |      3.74 KB |
| ResolveMultipleToMultiple_UsingMultipleResolve |       4.692 μs |     0.0552 μs |     0.0489 μs |      3.61 KB |
