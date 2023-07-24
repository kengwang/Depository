using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class NullableConstructorInjectService : IConstructorInjectService
{
    public NullableConstructorInjectService(IGuidGenerator? guidGenerator = null)
    {
        
    }
}