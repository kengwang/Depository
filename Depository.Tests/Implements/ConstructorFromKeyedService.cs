using Depository.Abstraction.Attributes;
using Depository.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Depository.Tests.Implements;

public class ConstructorFromKeyedService : ICheckIsNormal
{
    public readonly IGuidGenerator GuidGenerator;
    public ConstructorFromKeyedService([FromKeyedServices("b")] IGuidGenerator guidGenerator)
    {
        GuidGenerator = guidGenerator;
        IsNormal = true;
    }

    public bool IsNormal { get; set; }
}