namespace Depository.Core;

public partial class Depository
{
    public void AddImplementation(Type implementType, object implement)
    {
        _rootScope.AddImplement(implementType, implement);
    }

    public void RemoveImplementation(Type implementType, object implement)
    {
        _rootScope.RemoveImplement(implementType, implement);
    }

    public void RemoveAllImplementation(Type implementType)
    {
        _rootScope.RemoveAllImplements(implementType);
    }

    public void SetImplementation(Type implementType, object implement)
    {
        _rootScope.SetImplementation(implementType, implement);
    }
}