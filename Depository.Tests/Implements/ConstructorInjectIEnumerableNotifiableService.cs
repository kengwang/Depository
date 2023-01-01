using Depository.Abstraction.Interfaces;
using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class ConstructorInjectIEnumerableNotifiableService : IConstructorInjectService, INotifyDependencyChanged
{
    private readonly IEnumerable<IGuidGenerator> _generators;

    public ConstructorInjectIEnumerableNotifiableService(IEnumerable<IGuidGenerator> generators)
    {
        _generators = generators;
        DependencyChanged += (_, _) => IsNormal = true;
    }

    public bool IsNormal { get; set; }
    public DependencyChangedEventHandler? DependencyChanged { get; set; }
}