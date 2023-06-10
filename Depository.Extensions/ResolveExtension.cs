using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models.Options;

namespace Depository.Extensions;

public static class ResolveExtension
{
    public static async Task<T> ResolveAsync<T>(this IDepositoryResolve depository, DependencyResolveOption? option)
    {
        return (T)await depository.ResolveDependencyAsync(typeof(T), option);
    }
    
    public static async Task<T> ResolveAsync<T>(this IDepositoryResolve depository, string? relationName = null, bool? includeDisabled = false, IDepositoryResolveScope? scope = null, Dictionary<Type,object>? fatherImplementations = null)
    {
        if (relationName != null || includeDisabled == true || scope != null || fatherImplementations != null)
        {
            var option = new DependencyResolveOption
            {
                Scope = scope,
                IncludeDisabled = includeDisabled is true,
                RelationName = relationName,
                FatherImplementations = fatherImplementations
            };
            return (T)await depository.ResolveDependencyAsync(typeof(T), option);
        }
        return (T)await depository.ResolveDependencyAsync(typeof(T));

    }
    
    public static async Task<List<T>> ResolveMultipleAsync<T>(this IDepositoryResolve depository, DependencyResolveOption? option)
    {
        return (await depository.ResolveDependenciesAsync(typeof(T)))
            .Select(o => (T)o)
            .ToList();
    }
    
    public static async Task<List<T>> ResolveMultipleAsync<T>(this IDepositoryResolve depository, string? relationName = null, bool? includeDisabled = false, IDepositoryResolveScope? scope = null, Dictionary<Type,object>? fatherImplementations = null)
    {
        if (relationName != null || includeDisabled == true || scope != null || fatherImplementations != null)
        {
            var option = new DependencyResolveOption
            {
                Scope = scope,
                IncludeDisabled = includeDisabled is true,
                RelationName = relationName,
                FatherImplementations = fatherImplementations
            };
            return (await depository.ResolveDependenciesAsync(typeof(T), option))
                .Select(o => (T)o)
                .ToList();
        }

        return (await depository.ResolveDependenciesAsync(typeof(T)))
            .Select(o => (T)o)
            .ToList();
    }
}