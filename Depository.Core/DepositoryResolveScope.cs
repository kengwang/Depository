using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;

namespace Depository.Core;

public class DepositoryResolveScope : IDepositoryResolveScope
{
    // ReSharper disable once UnusedMember.Global
    public static IDepositoryResolveScope Create()
    {
        return new DepositoryResolveScope();
    }
    
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
        // Dispose it before remove it
        if (_implementations[type].Target is IDisposable disposableTarget)
        {
            disposableTarget.Dispose();
        }

        _implementations.Remove(type);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        foreach (var weakReference in _implementations.Values.ToList())
        {
            if (weakReference.Target is IDisposable disposableTarget)
            {
                disposableTarget.Dispose();
            }
        }
    }
}