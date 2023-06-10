namespace Depository.Abstraction.Interfaces;

public interface IDepositoryFamily
{
    public Task<List<object>> GetChildren(object father);
    public Task<List<object>> GetParents(object child);
}