using Depository.Abstraction.Interfaces;
using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class ConstructorInjectNotifiableService : IConstructorInjectService, INotifyDependencyChanged<IGuidGenerator>, ICheckIsNormal
{

    private readonly IGuidGenerator _generator;
    
    public ConstructorInjectNotifiableService(IGuidGenerator generator)
    {
        _generator = generator;
    }

    public bool IsNormal { get; set; }
    public Task OnDependencyChanged(IGuidGenerator? marker)
    {
        IsNormal = true;
        return Task.CompletedTask;
    }
}