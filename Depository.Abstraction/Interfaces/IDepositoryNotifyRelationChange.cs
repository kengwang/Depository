namespace Depository.Abstraction.Interfaces;

public interface IDepositoryNotifyRelationChange
{
    public Task NotifyRelationChange(Type changedType);
}