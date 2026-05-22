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
        return (T)depository.ResolveDependency(typeof(T), CreateScopedOption(scope, option));
    }

    public static T Resolve<T>(this IDepositoryResolve depository, string? relationName = null,
        bool? includeDisabled = false, IDepositoryResolveScope? scope = null,
        Dictionary<Type, Dictionary<string, object>>? fixedImplementations = null, bool checkAsyncConstruct = true)
    {
        if (relationName != null || includeDisabled == true || scope != null || fixedImplementations != null || checkAsyncConstruct == false)
        {
            var option = new DependencyResolveOption
            {
                Scope = scope,
                IncludeDisabled = includeDisabled is true,
                RelationName = relationName,
                FixedImplementations = fixedImplementations,
                CheckAsyncConstructor = checkAsyncConstruct
            };
            return (T)depository.ResolveDependency(typeof(T), option);
        }

        return (T)depository.ResolveDependency(typeof(T));
    }

    public static List<T> ResolveMultiple<T>(this IDepositoryResolve depository,
        DependencyResolveOption? option)
    {
        if (depository is Core.Depository concreteDepository)
            return concreteDepository.ResolveDependencies<T>(option);

        return depository.ResolveDependencies(typeof(T), option)
            .Select(o => (T)o)
            .ToList();
    }

    public static List<T> ResolveMultipleInScope<T>(this IDepositoryResolve depository, IDepositoryResolveScope scope,
        DependencyResolveOption? option = null)
    {
        var scopedOption = CreateScopedOption(scope, option);
        if (depository is Core.Depository concreteDepository)
            return concreteDepository.ResolveDependencies<T>(scopedOption);

        return depository.ResolveDependencies(typeof(T), scopedOption)
            .Select(o => (T)o)
            .ToList();
    }

    public static List<T> ResolveMultiple<T>(this IDepositoryResolve depository,
        string? relationName = null, bool? includeDisabled = false, IDepositoryResolveScope? scope = null,
        Dictionary<Type, Dictionary<string, object>>? fixedImplementations = null, bool checkAsyncConstruct = true)
    {
        if (relationName != null || includeDisabled == true || scope != null || fixedImplementations != null ||
            checkAsyncConstruct == false)
        {
            var option = new DependencyResolveOption
            {
                Scope = scope,
                IncludeDisabled = includeDisabled is true,
                RelationName = relationName,
                FixedImplementations = fixedImplementations,
                CheckAsyncConstructor = checkAsyncConstruct
            };
            if (depository is Core.Depository concreteDepository)
                return concreteDepository.ResolveDependencies<T>(option);

            return depository.ResolveDependencies(typeof(T), option)
                .Select(o => (T)o)
                .ToList();
        }

        if (depository is Core.Depository defaultConcreteDepository)
            return defaultConcreteDepository.ResolveDependencies<T>();

        return depository.ResolveDependencies(typeof(T))
            .Select(o => (T)o)
            .ToList();
    }

    private static DependencyResolveOption CreateScopedOption(IDepositoryResolveScope scope, DependencyResolveOption? option)
    {
        if (option is null)
        {
            return new DependencyResolveOption { Scope = scope };
        }

        return new DependencyResolveOption
        {
            Scope = scope,
            IncludeDisabled = option.IncludeDisabled,
            SkipDecoration = option.SkipDecoration,
            RelationName = option.RelationName,
            CheckAsyncConstructor = option.CheckAsyncConstructor,
            ThrowWhenNotExists = option.ThrowWhenNotExists,
            FixedImplementations = option.FixedImplementations
        };
    }
}
