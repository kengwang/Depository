using Depository.Abstraction.Interfaces;

namespace Depository.Extensions;

public static class ResolveExtension
{
    public static async Task<T> ResolveAsync<T>(this IDepositoryResolve depository)
    {
        return (T)await depository.ResolveDependencyAsync(typeof(T));
    }
    
    public static async Task<T> ResolveAsync<T>(this IDepositoryResolve depository, string relationName)
    {
        return (T)await depository.ResolveDependencyAsync(typeof(T));
    }
    
    public static async Task<List<T>> ResolveMultipleAsync<T>(this IDepositoryResolve depository)
    {
        return (await depository.ResolveDependenciesAsync(typeof(T)))
            .Select(o => (T)o)
            .ToList();
    }
}