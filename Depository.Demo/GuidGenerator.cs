using Depository.Abstraction.Interfaces;

namespace Depository.Demo;

public class GuidGenerator : IGuidGenerator, INotifyDependencyChanged
{
    private readonly Guid _guid = Guid.NewGuid();

    public Guid GetGuid() => _guid;
    public int GetRandom() => _randomProvider.GetRandomNumber();

    private IRandomProvider _randomProvider;

    public GuidGenerator(IRandomProvider randomProvider)
    {
        _randomProvider = randomProvider;
        DependencyChanged += (type, val) =>
        {
            if (type == typeof(IRandomProvider))
            {
                _randomProvider = (IRandomProvider)val;
            }
        };
    }

    public DependencyChangedEventHandler? DependencyChanged { get; }
}