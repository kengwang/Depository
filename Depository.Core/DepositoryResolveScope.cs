using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;

namespace Depository.Core;

public class DepositoryResolveScope : IDepositoryResolveScope
{
    private readonly Dictionary<Type, WeakReference> _implementations = new();

    public Task SetImplementAsync(Type type, object impl)
    {
        _implementations[type] = new WeakReference(impl);
        return Task.CompletedTask;
    }

    public Task<object> GetImplementAsync(Type type)
    {
        _implementations.TryGetValue(type, out var implRef);
        if (implRef?.Target is null) throw new ImplementNotFoundException();
        return Task.FromResult(implRef.Target);
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