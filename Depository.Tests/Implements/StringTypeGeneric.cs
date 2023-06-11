using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class StringTypeGeneric : ITypeGeneric<string>
{
    public Type GetGenericType() => typeof(string);
}