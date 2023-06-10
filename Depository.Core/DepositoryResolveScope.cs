using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public class DepositoryResolveScope : IDepositoryResolveScope
{
    private readonly DepositoryResolveScopeOption? _option;

    // ReSharper disable once UnusedMember.Global
    public static IDepositoryResolveScope Create(DepositoryResolveScopeOption? option = null)
    {
        return new DepositoryResolveScope(option);
    }

    private readonly Dictionary<Type, List<WeakReference>> _implementations = new();

    public DepositoryResolveScope(DepositoryResolveScopeOption? option = null)
    {
        _option = option;
    }

    public Task SetImplementationAsync(Type type, object? impl)
    {
        _implementations.TryGetValue(type, out var implList);
        if (implList is null)
        {
            implList = new List<WeakReference>();
            _implementations[type] = implList;
        }

        implList.RemoveAll(t => t.Target == impl);
        implList.Add(new WeakReference(impl));
        return Task.CompletedTask;
    }

    public Task<List<object>?> GetImplementsAsync(Type type)
    {
        _implementations.TryGetValue(type, out var implRefList);
        if (implRefList is null)
            return Task.FromResult<List<object>?>(null);

        return Task.FromResult(implRefList.Select(i => i.Target).ToList())!;
    }

    public Task<object?> GetImplementAsync(Type type)
    {
        _implementations.TryGetValue(type, out var implRefList);
        if (implRefList is not { Count: > 0 })
            return Task.FromResult<object?>(null);

        return Task.FromResult(implRefList[implRefList.Count - 1].Target);
    }

    public Task AddImplementAsync(Type type, object impl)
    {
        _implementations.TryGetValue(type, out var implList);
        if (implList is null)
        {
            implList = new List<WeakReference>();
            _implementations[type] = implList;
        }
        implList.Add(new WeakReference(impl));
        return Task.CompletedTask;
    }

    public Task<bool> ExistAsync(Type type)
    {
        _implementations.TryGetValue(type, out var implList);
        return Task.FromResult(implList is { Count: > 0 });
    }

    public Task RemoveAllImplementsAsync(Type type)
    {
        _implementations.TryGetValue(type, out var implList);
        if (implList is null) return Task.CompletedTask;
        // Dispose it before remove it
        if (_option?.AutoDisposeWhenRemoved is true)
            foreach (var weakReference in implList)
            {
                if (weakReference.Target is IDisposable disposableTarget)
                {
                    disposableTarget.Dispose();
                }
            }

        implList.Clear();
        _implementations.Remove(type);
        return Task.CompletedTask;
    }

    public Task RemoveImplementAsync(Type type, object implement)
    {
        _implementations.TryGetValue(type, out var implList);
        if (implList is null) return Task.CompletedTask;
        foreach (var weakReference in implList)
        {
            if (weakReference.Target == implement || weakReference.IsAlive == false || weakReference.Target is null) implList.Remove(weakReference);
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_option?.AutoDisposeWhenRemoved is true)
            foreach (var weakReferences in _implementations.Values.ToList())
            {
                foreach (var weakReference in weakReferences)
                {
                    if (weakReference.Target is IDisposable disposableTarget)
                    {
                        disposableTarget.Dispose();
                    }
                }
            }

        _implementations.Clear();
    }
}