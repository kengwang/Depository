using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class EmptyGuidGenerator : IGuidGenerator
{
    public Guid GetGuid() => Guid.Empty;
}