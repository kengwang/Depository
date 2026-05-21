namespace Depository.Core;

public partial class Depository
{
    public void RemoveImplementation(Type implementType, string? key = null)
    {
        RootScope.RemoveImplement(implementType, key);
    }


    public void SetImplementation(Type implementType, object implement, string? key = null)
    {
        RootScope.SetImplementation(implementType, implement);
    }
}