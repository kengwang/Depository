namespace Depository.Abstraction.Exceptions;

public class DependencyNotFoundException : DepositoryResolveException
{
    public Type DependencyType { get; set; } = null!;
}