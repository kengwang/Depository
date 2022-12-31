using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class RandomGuidGenerator : IGuidGenerator
{
    public Guid GetGuid() => Guid.NewGuid();
}