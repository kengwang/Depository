using System.Collections;
using System.Runtime.CompilerServices;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    private readonly ConditionalWeakTable<object, List<Type>> _resolvedTypes = new();
    private readonly Dictionary<Type, List<WeakReference>> _usedImpls = new();
    
    private async Task NotifyDependencyChange(DependencyDescription dependencyDescription,
        DependencyRelation relation)
    {
        // 回溯使用了此 Relation 的东西
        if (!_usedImpls.TryGetValue(dependencyDescription.DependencyType, out var impls)) return;
        foreach (var weakReference in impls.FindAll(t => t.IsAlive && t.Target is INotifyDependencyChanged)
                     .ToList())
        {
            var newImpl =
                await ResolveRelationAsync(dependencyDescription, relation,
                    new DependencyResolveOption()); // TODO: 可能之后会有其他的 Option 优化此处
            ((INotifyDependencyChanged)weakReference.Target).DependencyChanged?
                .Invoke(dependencyDescription.DependencyType, newImpl);
        }
    }


    public async Task<List<object>> ResolveDependenciesAsync(Type dependency, DependencyResolveOption? option)
    {
        if (dependency.IsGenericType && _dependencyDescriptions.All(t => t.DependencyType != dependency))
        {
            return await ResolveGenericDependencies(dependency, option);
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        var relations = await GetRelationsAsync(dependencyDescription);
        List<object> results = new();
        foreach (var relation in relations)
        {
            var result = await ResolveRelationAsync(dependencyDescription, relation, option);
            results.Add(result);
        }

        return results;
    }

    public async Task<object> ResolveDependencyAsync(Type dependency, DependencyResolveOption? option = null)
    {
        if (dependency.IsGenericType)
        {
            if (dependency.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                // check whether is IEnumerable
                // and then return the fully Implemented stuff
                var cachedGenericType = dependency.GenericTypeArguments[0];
                var resolves = (await ResolveDependenciesAsync(cachedGenericType, option));
                var impls = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(cachedGenericType));
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var impl in resolves)
                {
                    if (impl is null) continue;
                    if (cachedGenericType.IsInstanceOfType(impl))
                        impls.Add(impl);
                }

                return impls;
            }
            // ReSharper disable once RedundantIfElseBlock
            else
            {
                // normal open-generic type
                // check if is implemented as an existed generic
                if (_dependencyDescriptions.All(t => t.DependencyType != dependency))
                {
                    return await ResolveGenericDependency(dependency, option);
                }
            }
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        var relation = await GetRelationAsync(dependencyDescription);
        return await ResolveRelationAsync(dependencyDescription, relation, option);
    }

    private async Task<object> ResolveGenericDependency(Type dependency, DependencyResolveOption? option)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        var relation = await GetRelationAsync(dependencyDescription);
        if (relation.DefaultImplementation is not null) return relation.DefaultImplementation;
        var implementType = relation.ImplementType;
        if (!dependency.ContainsGenericParameters)
            implementType = relation.ImplementType.MakeGenericType(dependency.GenericTypeArguments);
        return dependencyDescription.Lifetime switch
        {
            DependencyLifetime.Singleton => await ResolveSingleton(implementType, option),
            DependencyLifetime.Transient => await ResolveTransient(implementType, option),
            DependencyLifetime.Scoped => await ResolveScope(implementType, option),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<List<object>> ResolveGenericDependencies(Type dependency, DependencyResolveOption? option)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        var relations = await GetRelationsAsync(dependencyDescription);
        List<object> results = new();
        foreach (var relation in relations)
        {
            if (relation.DefaultImplementation is not null) results.Add(relation.DefaultImplementation);
            var implementType = relation.ImplementType;
            if (!dependency.ContainsGenericParameters)
                implementType = relation.ImplementType.MakeGenericType(dependency.GenericTypeArguments);
            var impl = dependencyDescription.Lifetime switch
            {
                DependencyLifetime.Singleton => await ResolveSingleton(implementType, option),
                DependencyLifetime.Transient => await ResolveTransient(implementType, option),
                DependencyLifetime.Scoped => await ResolveScope(implementType, option),
                _ => throw new ArgumentOutOfRangeException()
            };
            results.Add(impl);
        }

        return results;
    }

    private async Task<object> ImplementActivator(Type implementType, DependencyResolveOption? option)
    {
        var constructorInfos = implementType.GetConstructors();
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (constructorInfos.Length == 0)
            throw new DependencyInitializationException($"Cannot initialize {implementType.Name} with no constructor");
        if (constructorInfos.Length > 1)
            throw new DependencyInitializationException(
                $"More than one constructor was founded in {implementType.Name}");
        var constructorInfo = constructorInfos[0];
        var parameterInfos = constructorInfo.GetParameters();
        var parameters = new List<object>();
        foreach (var parameterInfo in parameterInfos)
        {
            parameters.Add(await ResolveDependencyAsync(parameterInfo.ParameterType, option));
        }

        var dependencyImpl = constructorInfo.Invoke(parameters.ToArray());


        // Wire to the regenerator
        var weakRef = new WeakReference(dependencyImpl);
        var types = parameterInfos.Select(t => t.ParameterType).ToList();
        foreach (var type in types)
        {
            if (!_usedImpls.TryGetValue(type, out var references))
            {
                references = new List<WeakReference>();
                _usedImpls[type] = references;
            }

            references.Add(weakRef);
        }

        _resolvedTypes.GetOrCreateValue(dependencyImpl).AddRange(types);
        return dependencyImpl;
    }


    private async Task<object> ResolveRelationAsync(
        DependencyDescription dependencyDescription,
        DependencyRelation relation,
        DependencyResolveOption? option)
    {
        if (relation.DefaultImplementation is not null) return relation.DefaultImplementation;
        return dependencyDescription.Lifetime switch
        {
            DependencyLifetime.Singleton => await ResolveSingleton(relation.ImplementType, option),
            DependencyLifetime.Transient => await ResolveTransient(relation.ImplementType, option),
            DependencyLifetime.Scoped => await ResolveScope(relation.ImplementType, option),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<object> ResolveScope(Type implementType, DependencyResolveOption? option)
    {
        if (option?.Scope is null) throw new ScopeNotSetException();
        if (await option.Scope.ExistAsync(implementType))
            return await option.Scope.GetImplementAsync(implementType);
        var impl = await ImplementActivator(implementType, option);
        await option.Scope.SetImplementAsync(implementType, impl);
        return impl;
    }

    private async Task<object> ResolveTransient(Type implementType, DependencyResolveOption? option)
    {
        var impl = await ImplementActivator(implementType, option);
        return impl;
    }

    private async Task<object> ResolveSingleton(Type implementType, DependencyResolveOption? option)
    {
        if (await _rootScope.ExistAsync(implementType))
            return await _rootScope.GetImplementAsync(implementType);
        var impl = await ImplementActivator(implementType, option);
        await _rootScope.SetImplementAsync(implementType, impl);
        return impl;
    }

}