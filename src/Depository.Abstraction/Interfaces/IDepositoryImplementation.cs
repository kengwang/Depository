namespace Depository.Abstraction.Interfaces;

public interface IDepositoryImplementation
{
    public void RemoveImplementation(Type implementType, string? key = null);
    public void SetImplementation(Type implementType, object implement, string? key = null);
}