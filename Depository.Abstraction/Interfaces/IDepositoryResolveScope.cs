namespace Depository.Abstraction.Interfaces;

public interface IDepositoryResolveScope : IDisposable
{
    public Task SetImplementationAsync(Type type, object? impl);
    public Task<List<object>?> GetImplementsAsync(Type type);
    public Task<object?> GetImplementAsync(Type type);
    public Task<bool> ExistAsync(Type type);
    public Task RemoveAllImplementsAsync(Type type);
    public Task RemoveImplementAsync(Type type, object implement);
}