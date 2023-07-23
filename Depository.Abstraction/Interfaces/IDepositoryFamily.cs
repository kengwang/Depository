namespace Depository.Abstraction.Interfaces;

public interface IDepositoryFamily
{
    public List<object> GetChildren(object father);
    public List<object> GetParents(object child);
}