``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
|                                         Method |     Mean |     Error |    StdDev | Allocated |
|----------------------------------------------- |---------:|----------:|----------:|----------:|
|                            PublishNotification | 3.348 μs | 0.0646 μs | 0.0718 μs |   2.96 KB |
|                  PublishNotificationWithResult | 3.356 μs | 0.0658 μs | 0.0808 μs |   3.21 KB |
|                          ResolveSingleToSingle | 2.958 μs | 0.0573 μs | 0.0589 μs |   2.76 KB |
|                        ResolveMultipleToSingle | 3.348 μs | 0.0655 μs | 0.1038 μs |   3.13 KB |
|     ResolveMultipleToMultiple_UsingIEnumerable | 5.962 μs | 0.1177 μs | 0.1761 μs |   3.91 KB |
| ResolveMultipleToMultiple_UsingMultipleResolve | 4.556 μs | 0.0910 μs | 0.1545 μs |   3.77 KB |
