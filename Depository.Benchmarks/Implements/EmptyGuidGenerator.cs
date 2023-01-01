using Depository.Benchmarks.Interfaces;

namespace Depository.Benchmarks.Implements;

public class EmptyGuidGenerator : IGuidGenerator
{
    public Guid GetGuid() => Guid.Empty;
}