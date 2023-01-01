using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class TypeGeneric<T> : ITypeGeneric<T>
{
    public Type GetGenericType() => typeof(T);
}