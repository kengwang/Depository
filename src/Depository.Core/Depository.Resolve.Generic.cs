using System.Diagnostics.CodeAnalysis;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    private readonly Dictionary<ClosedGenericImplementationKey, Type> _closedGenericImplementations = new();
    [RequiresDynamicCode("Creating generic type instances dynamically is not compatible with NativeAOT when the instantiation cannot be statically analyzed")]
    private object ResolveGenericDependency(Type dependency, ResolveContext context)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        if (dependencyDescription is null)
            return context.ThrowWhenNotExists ? throw new DependencyNotFoundException(dependency) : null!;

        var relation = SelectRelation(dependencyDescription, context);
        return relation is null ? null! : ResolveRelation(dependencyDescription, relation, dependency, context);
    }

    [RequiresDynamicCode("Creating generic type instances dynamically is not compatible with NativeAOT when the instantiation cannot be statically analyzed")]
    private List<object> ResolveGenericDependencies(Type dependency, ResolveContext context)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        if (dependencyDescription is null)
            return context.ThrowWhenNotExists ? throw new DependencyNotFoundException(dependency) : new List<object>();

        if (!_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
            return new List<object>();

        var results = new List<object>(relations.Count);

        if (!context.SkipDecoration)
        {
            foreach (var relation in relations)
            {
                if (ShouldResolveRelation(relation, context) && relation.IsDecorationRelation)
                {
                    results.Add(ResolveRelation(dependencyDescription, relation, dependency, context));
                    return results;
                }
            }
        }

        foreach (var relation in relations)
        {
            if (!ShouldResolveRelation(relation, context)) continue;
            if (relation.IsDecorationRelation) continue;
            results.Add(ResolveRelation(dependencyDescription, relation, dependency, context));
        }

        return results;
    }

    private List<T> ResolveGenericTypedDependencies<T>(Type dependency, ResolveContext context)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        if (dependencyDescription is null)
            return context.ThrowWhenNotExists ? throw new DependencyNotFoundException(dependency) : new List<T>();

        if (!_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
            return new List<T>();

        var results = new List<T>(relations.Count);

        if (!context.SkipDecoration)
        {
            foreach (var relation in relations)
            {
                if (!ShouldResolveRelation(relation, context) || !relation.IsDecorationRelation) continue;
                var decorated = ResolveRelation(dependencyDescription, relation, dependency, context);
                if (decorated is T typedDecorated) results.Add(typedDecorated);
                return results;
            }
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

    private Type CloseGenericImplementation(Type dependency, Type implementType)
    {
        if (dependency.ContainsGenericParameters || !implementType.IsGenericTypeDefinition)
            return implementType;

        var key = new ClosedGenericImplementationKey(implementType, dependency);
        if (_closedGenericImplementations.TryGetValue(key, out var closedImplementType))
            return closedImplementType;

        closedImplementType = implementType.MakeGenericType(dependency.GenericTypeArguments);
        _closedGenericImplementations[key] = closedImplementType;
        return closedImplementType;
    }

    private readonly struct ClosedGenericImplementationKey : IEquatable<ClosedGenericImplementationKey>
    {
        public ClosedGenericImplementationKey(Type openImplementationType, Type closedDependencyType)
        {
            OpenImplementationType = openImplementationType;
            ClosedDependencyType = closedDependencyType;
        }

        private Type OpenImplementationType { get; }
        private Type ClosedDependencyType { get; }

        public bool Equals(ClosedGenericImplementationKey other)
        {
            return OpenImplementationType == other.OpenImplementationType &&
                   ClosedDependencyType == other.ClosedDependencyType;
        }

        public override bool Equals(object? obj)
        {
            return obj is ClosedGenericImplementationKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (OpenImplementationType.GetHashCode() * 397) ^ ClosedDependencyType.GetHashCode();
            }
        }
    }
}
