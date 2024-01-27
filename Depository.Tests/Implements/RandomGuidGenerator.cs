using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class RandomGuidGenerator : IGuidGenerator
{
    public Guid? GeneratedGuid;
    public Guid GetGuid() => GeneratedGuid ??= Guid.NewGuid();
}