using Depository.Abstraction.Interfaces;

namespace Depository.Abstraction.Models;

public class DepositoryResolveScope : IDepositoryResolveScope
{
    private readonly Dictionary<Type, object?> _implementations = new();

    public Task SetImplementAsync(Type type, object? impl)
    {
        _implementations[type] = impl;
        return Task.CompletedTask;
    }

    public Task<object?> GetImplementAsync(Type type)
    {
        return Task.FromResult(_implementations.TryGetValue(type, out var impl) ? impl : null);
    }

    public Task<bool> ExistAsync(Type type)
    {
        return Task.FromResult(_implementations.ContainsKey(type));
    }

    public Task RemoveImplementAsync(Type type)
    {
        _implementations.Remove(type);
        return Task.CompletedTask;
    }
}