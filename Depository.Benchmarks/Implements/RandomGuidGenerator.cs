using Depository.Benchmarks.Interfaces;

namespace Depository.Benchmarks.Implements;

public class RandomGuidGenerator : IGuidGenerator
{
    public Guid GetGuid() => Guid.NewGuid();
}