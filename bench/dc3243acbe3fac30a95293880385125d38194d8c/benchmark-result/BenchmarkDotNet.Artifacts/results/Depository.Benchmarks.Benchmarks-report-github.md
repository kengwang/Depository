``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.306
  [Host]     : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2


```
|                                         Method |     Mean |     Error |    StdDev | Allocated |
|----------------------------------------------- |---------:|----------:|----------:|----------:|
|                            PublishNotification | 6.622 μs | 0.1302 μs | 0.1447 μs |   3.86 KB |
|                  PublishNotificationWithResult | 6.655 μs | 0.1282 μs | 0.1259 μs |    3.8 KB |
|                          ResolveSingleToSingle | 5.669 μs | 0.1110 μs | 0.1233 μs |   3.23 KB |
|                        ResolveMultipleToSingle | 5.866 μs | 0.0986 μs | 0.0874 μs |   3.52 KB |
|     ResolveMultipleToMultiple_UsingIEnumerable | 8.027 μs | 0.1534 μs | 0.1507 μs |   4.08 KB |
| ResolveMultipleToMultiple_UsingMultipleResolve | 7.057 μs | 0.0897 μs | 0.0839 μs |   4.02 KB |
