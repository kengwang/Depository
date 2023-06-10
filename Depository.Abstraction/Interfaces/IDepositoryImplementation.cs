namespace Depository.Abstraction.Interfaces;

public interface IDepositoryImplementation
{
    public Task AddImplementation(Type implementType, object implement);
    public Task RemoveImplementation(Type implementType, object implement);
}