namespace Depository.Core;

public partial class Depository
{
    public void AddImplementation(Type implementType, object implement)
    {
        RootScope.AddImplement(implementType, implement);
    }

    public void RemoveImplementation(Type implementType, object implement)
    {
        RootScope.RemoveImplement(implementType, implement);
    }

    public void RemoveAllImplementation(Type implementType)
    {
        RootScope.RemoveAllImplements(implementType);
    }

    public void SetImplementation(Type implementType, object implement)
    {
        RootScope.SetImplementation(implementType, implement);
    }
}