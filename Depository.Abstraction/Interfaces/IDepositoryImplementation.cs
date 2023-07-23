namespace Depository.Abstraction.Interfaces;

public interface IDepositoryImplementation
{
    public void AddImplementation(Type implementType, object implement);
    public void RemoveImplementation(Type implementType, object implement);
    public void RemoveAllImplementation(Type implementType);
    public void SetImplementation(Type implementType, object implement);
}