using System.Diagnostics;
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

    private readonly Dictionary<string, WeakReference> _implementations = new();

    public DepositoryResolveScope(DepositoryResolveScopeOption? option = null)
    {
        _option = option;
    }

    private static string GetKey(Type type, string? key) => $"{type.FullName}:{key ?? ""}";
    
    public void SetImplementation(Type type, object? impl, string? key = null)
    {
        var implKey = GetKey(type, key);
        _implementations[implKey] = new WeakReference(impl);
    }

    public object? GetImplement(Type type, string? key = null)
    {
        var implKey = GetKey(type, key);
        _implementations.TryGetValue(implKey, out var impl);
        return impl?.Target;
    }
    
    public bool Exist(Type type, string? key = null)
    {
        if (type is null)
        {
            Console.WriteLine(111);
        }
        var implKey = GetKey(type, key);
        return _implementations.ContainsKey(implKey);
    }

    public void RemoveImplement(Type type, string? key = null)
    {
        var implKey = GetKey(type, key);
        _implementations.TryGetValue(implKey, out var impl);
        if (impl is null) return;
        _implementations.Remove(implKey);
    }

    public void Dispose()
    {
        if (_option?.AutoDisposeWhenRemoved is true)
            foreach (var weakReference in _implementations.Values.ToList())
            {
                if (weakReference.IsAlive && weakReference.Target is IDisposable disposable)
                    disposable.Dispose();
            }

        _implementations.Clear();
    }
}