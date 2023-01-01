using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class StringTypeGeneric : ITypeGeneric<string>
{
    public Type GetGenericType() => typeof(string);
}