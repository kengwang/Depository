namespace Depository.Abstraction.Interfaces;

public interface IDepositoryResolveScope : IDisposable
{
    public Task SetImplementAsync(Type type, object impl);
    public Task<object> GetImplementAsync(Type type);
    public Task<bool> ExistAsync(Type type);
    public Task RemoveImplementAsync(Type type);
}