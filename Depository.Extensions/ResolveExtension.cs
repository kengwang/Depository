using Depository.Abstraction.Interfaces;

namespace Depository.Extensions;

public static class ResolveExtension
{
    public static async Task<T> Resolve<T>(this IDepositoryResolve depository)
    {
        return (T)await depository.ResolveDependencyAsync(typeof(T));
    }
}