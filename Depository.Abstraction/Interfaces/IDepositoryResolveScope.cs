namespace Depository.Abstraction.Interfaces;

public interface IDepositoryResolveScope : IDisposable
{
    public void SetImplementation(Type type, object? impl);
    public List<object>? GetImplements(Type type);
    public object? GetImplement(Type type);
    public void AddImplement(Type type, object impl);
    public bool Exist(Type type);
    public void RemoveAllImplements(Type type);
    public void RemoveImplement(Type type, object implement);
}