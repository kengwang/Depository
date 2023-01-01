using Depository.Benchmarks.Interfaces;

namespace Depository.Benchmarks.Implements;

public class FooGuidGenerator<T> : IGuidGenerator
{
    public Guid GetGuid() => Guid.NewGuid();
}