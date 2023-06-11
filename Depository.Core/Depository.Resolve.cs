using System.Collections;
using System.Runtime.CompilerServices;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public partial class Depository
{
    private async Task NotifyDependencyChange(DependencyDescription dependencyDescription)
    {
        var notificationType = typeof(INotifyDependencyChanged<>).MakeGenericType(dependencyDescription.DependencyType);
        var description = GetDependencyDescription(notificationType);
        if (description is null) return;
        var relations = await GetRelationsAsync(description);
        foreach (var relation in relations)
        {
            var result = await ResolveRelationAsync(description, relation);
            notificationType.GetMethods()[0].Invoke(result, new object?[] { null });
        }
    }


    public async Task<List<object>> ResolveDependenciesAsync(Type dependency, DependencyResolveOption? option = null)
    {
        if (dependency.IsGenericType && _dependencyDescriptions.All(t => t.DependencyType != dependency))
        {
            return await ResolveGenericDependencies(dependency, option);
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        if (dependencyDescription is null) throw new DependencyNotFoundException(dependency);
        var relations = await GetRelationsAsync(dependencyDescription, option?.IncludeDisabled is true);
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
        if (dependencyDescription is null) throw new DependencyNotFoundException(dependency);
        var relation = await GetRelationAsync(dependencyDescription, option?.IncludeDisabled is true);
        return await ResolveRelationAsync(dependencyDescription, relation, option);
    }

    public async Task ChangeResolveTargetAsync(Type dependency, object? target)
    {
        var description = GetDependencyDescription(dependency);
        if (description is null) throw new DependencyNotFoundException(dependency);
        if (description.Lifetime == DependencyLifetime.Singleton)
        {
            await _rootScope.SetImplementationAsync(dependency, target);
        }

        if (_option.AutoNotifyDependencyChange)
            await NotifyDependencyChange(description);
    }

    private async Task<object> ResolveGenericDependency(Type dependency, DependencyResolveOption? option)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        if (dependencyDescription is null) throw new DependencyNotFoundException(dependency);
        var relation = await GetRelationAsync(dependencyDescription, option?.IncludeDisabled is true);
        if (relation.DefaultImplementation is not null) return relation.DefaultImplementation;
        var implementType = relation.ImplementType;
        if (!dependency.ContainsGenericParameters)
            implementType = relation.ImplementType.MakeGenericType(dependency.GenericTypeArguments);
        return dependencyDescription.Lifetime switch
        {
            DependencyLifetime.Singleton => await ResolveSingleton(implementType, option),
            DependencyLifetime.Transient => await ResolveTransient(implementType, option),
            DependencyLifetime.Scoped => await ResolveScoped(implementType, option),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<List<object>> ResolveGenericDependencies(Type dependency, DependencyResolveOption? option)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        if (dependencyDescription is null) throw new DependencyNotFoundException(dependency);
        var relations = await GetRelationsAsync(dependencyDescription, option?.IncludeDisabled is true);
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
                DependencyLifetime.Scoped => await ResolveScoped(implementType, option),
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
            if (option?.FatherImplementations?.TryGetValue(parameterInfo.ParameterType, out var impl) is true)
            {
                parameters.Add(impl);
            }
            else
            {
                parameters.Add(await ResolveDependencyAsync(parameterInfo.ParameterType, option));
            }
        }

        var dependencyImpl = constructorInfo.Invoke(parameters.ToArray());
        if (option?.FatherImplementations is { Count: > 0 })
        {
            foreach (var kvp in option.FatherImplementations)
            {
                _fatherToChildRelation.GetOrCreateValue(kvp.Value).Add(new WeakReference(dependencyImpl));
                _childToFatherRelation.GetOrCreateValue(dependencyImpl).Add(new WeakReference(kvp.Value));
            }
        }

        if (option?.CheckAsyncConstructor is not false && dependencyImpl is IAsyncConstructService asyncConstructService)
            await asyncConstructService.InitializeService();
        return dependencyImpl;
    }


    private async Task<object> ResolveRelationAsync(
        DependencyDescription dependencyDescription,
        DependencyRelation relation,
        DependencyResolveOption? option = null)
    {
        if (relation.DefaultImplementation is not null) return relation.DefaultImplementation;
        return dependencyDescription.Lifetime switch
        {
            DependencyLifetime.Singleton => await ResolveSingleton(relation.ImplementType, option),
            DependencyLifetime.Transient => await ResolveTransient(relation.ImplementType, option),
            DependencyLifetime.Scoped => await ResolveScoped(relation.ImplementType, option),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<object> ResolveScoped(Type implementType, DependencyResolveOption? option)
    {
        if (option?.Scope is null) throw new ScopeNotSetException();
        if (await option.Scope.ExistAsync(implementType))
        {
            var ret = await option.Scope.GetImplementAsync(implementType);
            if (ret is not null) return ret;
        }
        var impl = await ImplementActivator(implementType, option);
        await option.Scope.SetImplementationAsync(implementType, impl);
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
        {
            var ret = await _rootScope.GetImplementAsync(implementType);
            if (ret is not null) return ret;
        }
        var impl = await ImplementActivator(implementType, option);
        await _rootScope.SetImplementationAsync(implementType, impl);
        return impl;
    }
}