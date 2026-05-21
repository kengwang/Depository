using System.Diagnostics.CodeAnalysis;

namespace Depository.Abstraction.Interfaces;

public interface IDepositoryResolveScope : IDisposable
{
    public void SetImplementation([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, object? impl, string? key = null);
    public object? GetImplement(Type type, string? key = null);
    public bool Exist(Type type, string? key = null);
    public void RemoveImplement([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, string? key = null);
}
