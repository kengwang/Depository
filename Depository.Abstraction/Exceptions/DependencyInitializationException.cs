namespace Depository.Abstraction.Exceptions;

public class DependencyInitializationException : DepositoryResolveException
{
    public DependencyInitializationException()
    {
        
    }
    
    public DependencyInitializationException(string msg) : base(msg)
    {
        
    }
}