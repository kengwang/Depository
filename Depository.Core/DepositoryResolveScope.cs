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

    public void SetImplementation(Type type, object? impl)
    {
        _implementations.TryGetValue(type, out var implList);
        if (implList is null)
        {
            implList = new List<WeakReference>();
            _implementations[type] = implList;
        }

        implList.RemoveAll(t => t.Target == impl);
        implList.Add(new WeakReference(impl));
    }

    public List<object>? GetImplements(Type type)
    {
        _implementations.TryGetValue(type, out var implRefList);
        return implRefList?.Select(i => i.Target).ToList();
    }

    public object? GetImplement(Type type)
    {
        _implementations.TryGetValue(type, out var implRefList);
        return implRefList is not { Count: > 0 } ? null : implRefList[implRefList.Count - 1].Target;
    }

    public void AddImplement(Type type, object impl)
    {
        _implementations.TryGetValue(type, out var implList);
        if (implList is null)
        {
            implList = new List<WeakReference>();
            _implementations[type] = implList;
        }
        implList.Add(new WeakReference(impl));
    }

    public bool Exist(Type type)
    {
        _implementations.TryGetValue(type, out var implList);
        return implList is { Count: > 0 };
    }

    public void RemoveAllImplements(Type type)
    {
        _implementations.TryGetValue(type, out var implList);
        if (implList is null) return;
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
    }

    public void RemoveImplement(Type type, object implement)
    {
        _implementations.TryGetValue(type, out var implList);
        if (implList is null) return;
        foreach (var weakReference in implList)
        {
            if (weakReference.Target == implement || weakReference.IsAlive == false || weakReference.Target is null) implList.Remove(weakReference);
        }
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