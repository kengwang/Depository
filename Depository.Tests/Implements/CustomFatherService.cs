using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class CustomFatherService : IConstructorInjectService
{
    public CustomFatherService(IGuidGenerator generator)
    {
        
    }
}