``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.102
  [Host]     : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2


```
|                                         Method |     Mean |     Error |    StdDev | Allocated |
|----------------------------------------------- |---------:|----------:|----------:|----------:|
|                            PublishNotification | 4.064 μs | 0.0501 μs | 0.0468 μs |   3.16 KB |
|                  PublishNotificationWithResult | 4.141 μs | 0.0605 μs | 0.0505 μs |   3.41 KB |
|                          ResolveSingleToSingle | 3.644 μs | 0.0617 μs | 0.0577 μs |   2.96 KB |
|                        ResolveMultipleToSingle | 3.967 μs | 0.0523 μs | 0.0489 μs |   3.33 KB |
|     ResolveMultipleToMultiple_UsingIEnumerable | 5.790 μs | 0.0765 μs | 0.0716 μs |   3.94 KB |
| ResolveMultipleToMultiple_UsingMultipleResolve | 4.814 μs | 0.0724 μs | 0.0677 μs |    3.8 KB |
