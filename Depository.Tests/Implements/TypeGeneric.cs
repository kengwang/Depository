using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class TypeGeneric<T> : ITypeGeneric<T>
{
    public Type GetGenericType() => typeof(T);
}