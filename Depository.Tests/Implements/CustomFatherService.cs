using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class CustomFatherService : IConstructorInjectService
{
    public CustomFatherService(IGuidGenerator generator)
    {
        
    }
}