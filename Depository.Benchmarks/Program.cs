// See https://aka.ms/new-console-template for more information

using System.Reflection;
using BenchmarkDotNet.Running;
using Depository.Benchmarks;

Console.WriteLine(@"
Preface

=== Chinese Version

此 Benchmark 仅为对于性能的测试并提供作为优化的方向
此 Benchmark 仅为基准测试，不代表最终情况
此 Benchmark 不是作者想要显示自己的有多厉害，而是为了明白自己和大佬们的差距
本 Benchmark 仅图一乐，不要过分解读！
如果有意见，请使用其他的 IoC 容器！Depository 就是个垃圾

=== English Version

This Benchmark is only a performance test and provides a direction for optimization
This Benchmark is a benchmark only and does not represent the final situation
This Benchmark is not for the author to show how powerful the Depository is, but to understand the gap between himself and the great programmers
This Benchmark is just for fun, don't over-interpret it!
If in doubt, use another IoC container! Depository is garbage
");

var summaries = BenchmarkSwitcher
    .FromAssembly(Assembly.GetCallingAssembly())
    .RunAll();