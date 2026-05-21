namespace Depository.Abstraction.Exceptions;

public class DependencyNotFoundException : DepositoryResolveException
{
    public DependencyNotFoundException(Type dependencyType)
    {
        DependencyType = dependencyType;
    }

    public Type DependencyType { get; }
}