using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class EmptyGuidGenerator : IGuidGenerator
{
    public Guid GetGuid() => Guid.Empty;
}