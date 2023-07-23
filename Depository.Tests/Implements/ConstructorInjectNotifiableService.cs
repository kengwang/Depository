using Depository.Abstraction.Interfaces;
using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class ConstructorInjectNotifiableService : IConstructorInjectService, INotifyDependencyChanged<IGuidGenerator>, ICheckIsNormal
{

    private readonly IGuidGenerator _generator;
    
    public ConstructorInjectNotifiableService(IGuidGenerator generator)
    {
        _generator = generator;
    }

    public bool IsNormal { get; set; }
    public void OnDependencyChanged(IGuidGenerator? marker)
    {
        IsNormal = true;
    }
}