``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
|                                         Method |     Mean |     Error |    StdDev | Allocated |
|----------------------------------------------- |---------:|----------:|----------:|----------:|
|                            PublishNotification | 2.313 μs | 0.0254 μs | 0.0225 μs |   2.96 KB |
|                  PublishNotificationWithResult | 2.339 μs | 0.0182 μs | 0.0152 μs |   3.21 KB |
|                          ResolveSingleToSingle | 2.198 μs | 0.0403 μs | 0.0377 μs |   2.76 KB |
|                        ResolveMultipleToSingle | 2.369 μs | 0.0091 μs | 0.0085 μs |   3.13 KB |
|     ResolveMultipleToMultiple_UsingIEnumerable | 4.152 μs | 0.0244 μs | 0.0229 μs |   3.91 KB |
| ResolveMultipleToMultiple_UsingMultipleResolve | 3.168 μs | 0.0614 μs | 0.0575 μs |   3.77 KB |
