namespace Depository.Abstraction.Interfaces;

public interface IDepositoryFamily
{
    public Task<List<object>> GetChildrenAsync(object father);
    public Task<List<object>> GetParentsAsync(object child);
}