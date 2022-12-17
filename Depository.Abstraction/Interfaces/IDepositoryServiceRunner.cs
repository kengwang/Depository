namespace Depository.Abstraction.Interfaces;

public interface IDepositoryServiceRunner
{
    public Task RunAsync(Type serviceType);
}