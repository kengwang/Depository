using Depository.Abstraction.Interfaces;
using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class ConstructorInjectNotifiableService : IConstructorInjectService, INotifyDependencyChanged
{

    private readonly IGuidGenerator _generator;
    
    public ConstructorInjectNotifiableService(IGuidGenerator generator)
    {
        _generator = generator;
        DependencyChanged += (_, _) => IsNormal = true;
    }
    public DependencyChangedEventHandler? DependencyChanged { get; set; }
    public bool IsNormal { get; set; }
}