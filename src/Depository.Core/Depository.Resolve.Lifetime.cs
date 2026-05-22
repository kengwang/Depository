using System.Diagnostics.CodeAnalysis;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    private object ResolveDescriptionWithImplementType(
        DependencyDescription description,
        DependencyRelation relation,
        Type inputType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        ResolveContext context)
    {
        var impl = description.Lifetime switch
        {
            DependencyLifetime.Singleton => ResolveSingleton(implementType, context),
            DependencyLifetime.Transient => ResolveTransient(implementType, context),
            DependencyLifetime.Scoped => ResolveScoped(implementType, context),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (context.CheckAsyncConstructor && impl is IAsyncConstructService asyncConstructService)
        {
            _ = asyncConstructService.InitializeService();
        }

        return impl;
    }

    private object ResolveScoped(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        ResolveContext context)
    {
        var scope = context.Scope ?? CurrentScope;
        if (scope is null) throw new ScopeNotSetException();
        if (TryGetCachedImplementation(scope, implementType, context.RelationName, out var cachedScoped) &&
            cachedScoped is not null)
            return cachedScoped;

        var impl = ResolveTypeToObject(implementType, context);
        scope.SetImplementation(implementType, impl, context.RelationName);
        return impl;
    }

    private object ResolveTransient(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        ResolveContext context)
    {
        return ResolveTypeToObject(implementType, context);
    }

    private object ResolveSingleton(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
        ResolveContext context)
    {
        if (TryGetCachedImplementation(RootScope, implementType, context.RelationName, out var cachedSingleton) &&
            cachedSingleton is not null)
            return cachedSingleton;

        var impl = ResolveTypeToObject(implementType, context);
        RootScope.SetImplementation(implementType, impl, context.RelationName);
        return impl;
    }


    private static bool TryGetCachedImplementation(
        IDepositoryResolveScope scope,
        Type implementType,
        string? relationName,
        out object? implementation)
    {
        if (scope is DepositoryResolveScope depositoryScope)
            return depositoryScope.TryGetImplement(implementType, relationName, out implementation);

        if (scope.Exist(implementType, relationName))
        {
            implementation = scope.GetImplement(implementType, relationName);
            return true;
        }

        implementation = null;
        return false;
    }

    internal static string SafeToString(object? obj)
    {
        if (obj == null)
            return "null";

        if (obj is string value)
            return value;

        var type = obj.GetType();
        var toStringMethod = type.GetMethod(nameof(ToString), Type.EmptyTypes);
        return toStringMethod != null && toStringMethod.DeclaringType != typeof(object)
            ? obj.ToString()!
            : $"{type.FullName}@{obj.GetHashCode():X}";
    }
}
