namespace Depository.Demo;

public class TypeGeneric<T> : ITypeGeneric<T>
{
    public string GetTypeName()
    {
        return nameof(T);
    }
}