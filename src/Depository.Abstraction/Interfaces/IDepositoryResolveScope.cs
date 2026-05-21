namespace Depository.Abstraction.Interfaces;

public interface IDepositoryResolveScope : IDisposable
{
    public void SetImplementation(Type type, object? impl, string? key = null);
    public object? GetImplement(Type type, string? key = null);
    public bool Exist(Type type, string? key = null);
    public void RemoveImplement(Type type, string? key = null);
}