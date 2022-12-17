namespace Depository.Abstraction.Interfaces;

public interface INotifyDependencyChanged
{
    public DependencyChangedEventHandler? DependencyChanged { get; }
}

public delegate void DependencyChangedEventHandler(Type changedType, object? implement);