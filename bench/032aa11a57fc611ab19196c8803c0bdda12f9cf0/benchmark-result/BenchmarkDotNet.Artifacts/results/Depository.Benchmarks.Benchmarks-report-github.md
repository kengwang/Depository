``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
|                                         Method |     Mean |     Error |    StdDev | Allocated |
|----------------------------------------------- |---------:|----------:|----------:|----------:|
|                            PublishNotification | 6.721 μs | 0.1322 μs | 0.2349 μs |   3.93 KB |
|                  PublishNotificationWithResult | 6.404 μs | 0.1262 μs | 0.1351 μs |   3.88 KB |
|                          ResolveSingleToSingle | 5.848 μs | 0.1142 μs | 0.1068 μs |   3.42 KB |
|                        ResolveMultipleToSingle | 6.308 μs | 0.0902 μs | 0.1074 μs |   3.79 KB |
|     ResolveMultipleToMultiple_UsingIEnumerable | 8.273 μs | 0.1622 μs | 0.2619 μs |   4.57 KB |
| ResolveMultipleToMultiple_UsingMultipleResolve | 7.916 μs | 0.1576 μs | 0.3359 μs |   4.44 KB |
