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

    private readonly Dictionary<ImplementationKey, object?> _implementations = new();

    public DepositoryResolveScope(DepositoryResolveScopeOption? option = null)
    {
        _option = option;
    }

    public void SetImplementation(Type type, object? impl, string? key = null)
    {
        _implementations[new ImplementationKey(type, key)] = impl;
    }

    public object? GetImplement(Type type, string? key = null)
    {
        _implementations.TryGetValue(new ImplementationKey(type, key), out var impl);
        return impl;
    }

    internal bool TryGetImplement(Type type, string? key, out object? impl)
    {
        return _implementations.TryGetValue(new ImplementationKey(type, key), out impl);
    }

    public bool Exist(Type type, string? key = null)
    {
        return _implementations.ContainsKey(new ImplementationKey(type, key));
    }

    public void RemoveImplement(Type type, string? key = null)
    {
        _implementations.Remove(new ImplementationKey(type, key));
    }

    public void Dispose()
    {
        if (_option?.AutoDisposeWhenRemoved is true)
            foreach (var implementation in _implementations.Values.ToList())
            {
                if (implementation is IDisposable disposable)
                    disposable.Dispose();
            }

        _implementations.Clear();
    }

    private readonly struct ImplementationKey : IEquatable<ImplementationKey>
    {
        public ImplementationKey(Type type, string? key)
        {
            Type = type;
            Key = key;
        }

        private Type Type { get; }
        private string? Key { get; }

        public bool Equals(ImplementationKey other)
        {
            return Type == other.Type && string.Equals(Key, other.Key, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is ImplementationKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode() * 397) ^ (Key is null ? 0 : StringComparer.Ordinal.GetHashCode(Key));
            }
        }
    }
}
