using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class RandomGuidGenerator : IGuidGenerator
{
    public Guid GetGuid() => Guid.NewGuid();
}