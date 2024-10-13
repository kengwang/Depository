using Depository.Abstraction.Attributes;
using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class ConstructorFromNamedService : ICheckIsNormal
{
    public readonly IGuidGenerator GuidGenerator;
    public ConstructorFromNamedService([FromNamedService("a")] IGuidGenerator guidGenerator)
    {
        GuidGenerator = guidGenerator;
        IsNormal = true;
    }

    public bool IsNormal { get; set; }
}