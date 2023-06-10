``` ini

BenchmarkDotNet=v0.13.3, OS=ubuntu 22.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
|                                         Method |     Mean |     Error |    StdDev | Allocated |
|----------------------------------------------- |---------:|----------:|----------:|----------:|
|                            PublishNotification | 5.658 μs | 0.1129 μs | 0.1255 μs |   3.63 KB |
|                  PublishNotificationWithResult | 5.936 μs | 0.1093 μs | 0.1074 μs |   3.88 KB |
|                          ResolveSingleToSingle | 5.271 μs | 0.1009 μs | 0.0944 μs |   3.42 KB |
|                        ResolveMultipleToSingle | 5.931 μs | 0.1150 μs | 0.1413 μs |   3.79 KB |
|     ResolveMultipleToMultiple_UsingIEnumerable | 7.669 μs | 0.1443 μs | 0.1544 μs |   4.57 KB |
| ResolveMultipleToMultiple_UsingMultipleResolve | 6.732 μs | 0.1260 μs | 0.1117 μs |   4.44 KB |
