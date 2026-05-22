using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public partial class Depository
{
    public List<object> ResolveDependencies(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependency,
        DependencyResolveOption? option = null)
    {
        return ResolveDependencies(dependency, ResolveContext.From(option));
    }

    private List<object> ResolveDependencies(Type dependency, ResolveContext context)
    {
        if (dependency.IsGenericType && !_dependencyDescriptionsByType.ContainsKey(dependency))
        {
            return ResolveGenericDependencies(dependency, context);
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        if (dependencyDescription is null)
            return context.ThrowWhenNotExists ? throw new DependencyNotFoundException(dependency) : new List<object>();

        if (!_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
            return new List<object>();

        var results = new List<object>(relations.Count);
        if (dependencyDescription.DecorationRelation is not null && !context.SkipDecoration)
        {
            results.Add(ResolveRelation(dependencyDescription, dependencyDescription.DecorationRelation, dependency, context));
            return results;
        }

        foreach (var relation in relations)
        {
            if (!ShouldResolveRelation(relation, context)) continue;
            if (relation.IsDecorationRelation) continue;
            results.Add(ResolveRelation(dependencyDescription, relation, dependency, context));
        }

        return results;
    }

    public List<T> ResolveDependencies<T>(DependencyResolveOption? option = null)
    {
        return ResolveTypedDependencies<T>(ResolveContext.From(option));
    }

    private List<T> ResolveTypedDependencies<T>(ResolveContext context)
    {
        var dependency = typeof(T);
        if (dependency.IsGenericType && !_dependencyDescriptionsByType.ContainsKey(dependency))
        {
            return ResolveGenericTypedDependencies<T>(dependency, context);
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        if (dependencyDescription is null)
            return context.ThrowWhenNotExists ? throw new DependencyNotFoundException(dependency) : new List<T>();

        if (!_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
            return new List<T>();

        var results = new List<T>(relations.Count);
        if (dependencyDescription.DecorationRelation is not null && !context.SkipDecoration)
        {
            var decorated = ResolveRelation(dependencyDescription, dependencyDescription.DecorationRelation, dependency, context);
            if (decorated is T typedDecorated) results.Add(typedDecorated);
            return results;
        }

        foreach (var relation in relations)
        {
            if (!ShouldResolveRelation(relation, context)) continue;
            if (relation.IsDecorationRelation) continue;
            var result = ResolveRelation(dependencyDescription, relation, dependency, context);
            if (result is T typedResult) results.Add(typedResult);
        }

        return results;
    }

    [RequiresDynamicCode("Creating generic type instances dynamically is not compatible with NativeAOT when the instantiation cannot be statically analyzed")]
    public object ResolveDependency(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependency,
        DependencyResolveOption? option = null)
    {
        return ResolveDependency(dependency, ResolveContext.From(option));
    }

    [RequiresDynamicCode("Creating generic type instances dynamically is not compatible with NativeAOT when the instantiation cannot be statically analyzed")]
    private object ResolveDependency(Type dependency, ResolveContext context)
    {
        if (dependency.IsGenericType)
        {
            var genericTypeDefinition = dependency.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(IEnumerable<>))
            {
                var itemType = dependency.GenericTypeArguments[0];
                if (!DependencyExist(itemType)) return Array.CreateInstance(itemType, 0);

                var resolvedImplementations = ResolveDependencies(itemType, context);
                var count = 0;
                foreach (var implementation in resolvedImplementations)
                {
                    if (implementation is not null && itemType.IsInstanceOfType(implementation))
                        count++;
                }

                var implementations = Array.CreateInstance(itemType, count);
                var index = 0;
                foreach (var implementation in resolvedImplementations)
                {
                    if (implementation is not null && itemType.IsInstanceOfType(implementation))
                        implementations.SetValue(implementation, index++);
                }

                return implementations;
            }

            if (genericTypeDefinition == typeof(Nullable<>))
            {
                var actualType = dependency.GenericTypeArguments[0];
                if (DependencyExist(actualType)) return ResolveDependency(actualType, context);
                return context.ThrowWhenNotExists ? throw new DependencyNotFoundException(actualType) : null!;
            }

            if (genericTypeDefinition == typeof(Task<>))
            {
                return ResolveTaskDependency(dependency, context);
            }

            if (!_dependencyDescriptionsByType.ContainsKey(dependency))
            {
                return ResolveGenericDependency(dependency, context);
            }
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        if (dependencyDescription is null)
            return context.ThrowWhenNotExists ? throw new DependencyNotFoundException(dependency) : null!;

        var relation = SelectRelation(dependencyDescription, context);
        return relation is null ? null! : ResolveRelation(dependencyDescription, relation, dependency, context);
    }

    [RequiresDynamicCode("Creating generic type instances dynamically is not compatible with NativeAOT when the instantiation cannot be statically analyzed")]
    private object ResolveTaskDependency(Type dependency, ResolveContext context)
    {
        var actualType = dependency.GenericTypeArguments[0];
        if (!DependencyExist(actualType))
        {
            return context.ThrowWhenNotExists ? throw new DependencyNotFoundException(actualType) : null!;
        }

        var result = ResolveDependency(actualType, context.WithCheckAsyncConstructor(false));
        if (result is not IAsyncConstructService asyncConstructService)
        {
            return typeof(Task).GetMethod(nameof(Task.FromResult))?.MakeGenericMethod(actualType)
                .Invoke(null, new[] { result })!;
        }

        return Task.Run(async () =>
        {
            await asyncConstructService.InitializeService();
            return asyncConstructService;
        });
    }

    public void ChangeResolveTarget(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependency,
        object? target)
    {
        var description = GetDependencyDescription(dependency);
        if (description is null) throw new DependencyNotFoundException(dependency);
        if (description.Lifetime == DependencyLifetime.Singleton)
        {
            RootScope.SetImplementation(dependency, target);
        }

        if (Option.AutoNotifyDependencyChange)
            NotifyDependencyChange(description);
    }
}
