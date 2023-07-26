using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models.Options;

namespace Depository.Extensions;

public static class ResolveExtension
{
    public static T Resolve<T>(this IDepositoryResolve depository, DependencyResolveOption? option)
    {
        return (T)depository.ResolveDependency(typeof(T), option);
    }

    public static T ResolveInScope<T>(this IDepositoryResolve depository, IDepositoryResolveScope scope, DependencyResolveOption? option = null)
    {
        option ??= new();
        option.Scope = scope;
        return (T)depository.ResolveDependency(typeof(T), option);
    }
    
    public static T Resolve<T>(this IDepositoryResolve depository, string? relationName = null,
        bool? includeDisabled = false, IDepositoryResolveScope? scope = null,
        Dictionary<Type, object>? fatherImplementations = null, bool checkAsyncConstruct = true)
    {
        if (relationName != null || includeDisabled == true || scope != null || fatherImplementations != null || checkAsyncConstruct == false)
        {
            var option = new DependencyResolveOption
            {
                Scope = scope,
                IncludeDisabled = includeDisabled is true,
                RelationName = relationName,
                FatherImplementations = fatherImplementations,
                CheckAsyncConstructor = checkAsyncConstruct
            };
            return (T)depository.ResolveDependency(typeof(T), option);
        }

        return (T)depository.ResolveDependency(typeof(T));
    }

    public static List<T> ResolveMultiple<T>(this IDepositoryResolve depository,
        DependencyResolveOption? option)
    {
        return (depository.ResolveDependencies(typeof(T)))
            .Select(o => (T)o)
            .ToList();
    }
    
    public static List<T> ResolveMultipleInScope<T>(this IDepositoryResolve depository, IDepositoryResolveScope scope,
                                             DependencyResolveOption? option = null)
    {
        option ??= new();
        option.Scope = scope;
        return (depository.ResolveDependencies(typeof(T)))
               .Select(o => (T)o)
               .ToList();
    }

    public static List<T> ResolveMultiple<T>(this IDepositoryResolve depository,
        string? relationName = null, bool? includeDisabled = false, IDepositoryResolveScope? scope = null,
        Dictionary<Type, object>? fatherImplementations = null, bool checkAsyncConstruct = true)
    {
        if (relationName != null || includeDisabled == true || scope != null || fatherImplementations != null ||
            checkAsyncConstruct == false)
        {
            var option = new DependencyResolveOption
            {
                Scope = scope,
                IncludeDisabled = includeDisabled is true,
                RelationName = relationName,
                FatherImplementations = fatherImplementations,
                CheckAsyncConstructor = checkAsyncConstruct
            };
            return (depository.ResolveDependencies(typeof(T), option))
                .Select(o => (T)o)
                .ToList();
        }

        return (depository.ResolveDependencies(typeof(T)))
            .Select(o => (T)o)
            .ToList();
    }
}