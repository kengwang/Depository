``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
|                                         Method |     Mean |     Error |    StdDev | Allocated |
|----------------------------------------------- |---------:|----------:|----------:|----------:|
|                            PublishNotification | 5.341 μs | 0.0937 μs | 0.0876 μs |   4.21 KB |
|                  PublishNotificationWithResult | 5.364 μs | 0.0450 μs | 0.0399 μs |   4.16 KB |
|                          ResolveSingleToSingle | 4.809 μs | 0.0662 μs | 0.0587 μs |   3.72 KB |
|                        ResolveMultipleToSingle | 5.101 μs | 0.0606 μs | 0.0567 μs |   4.09 KB |
|     ResolveMultipleToMultiple_UsingIEnumerable | 7.230 μs | 0.0758 μs | 0.0709 μs |   4.85 KB |
| ResolveMultipleToMultiple_UsingMultipleResolve | 6.311 μs | 0.0764 μs | 0.0677 μs |   4.72 KB |
