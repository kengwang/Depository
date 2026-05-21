namespace Depository.Abstraction.Interfaces;

public interface IDepositoryNotifyRelationChange
{
    public void NotifyRelationChange(Type changedType);
}