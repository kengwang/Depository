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
    private void NotifyDependencyChange(DependencyDescription dependencyDescription, int mode = 0)
    {
        if (mode is 0 or 1)
        {
            // Notify List
            PostTypeChangeNotification(
                typeof(IEnumerable<>).MakeGenericType(dependencyDescription.DependencyType));
        }

        if (mode is 0 or 2)
        {
            // Notify Single
            PostTypeChangeNotification(dependencyDescription.DependencyType);
        }
    }

    private void PostTypeChangeNotification(Type type)
    {
        var notificationType = typeof(INotifyDependencyChanged<>).MakeGenericType(type);
        var description = GetDependencyDescription(notificationType);
        if (description is null) return;
        var relations = GetRelations(description);
        foreach (var relation in relations)
        {
            var result = ResolveRelation(description, relation);
            notificationType.GetMethods()[0].Invoke(result, new object?[] { null });
        }
    }

    public List<object> ResolveDependencies(Type dependency, DependencyResolveOption? option = null)
    {
        if (dependency.IsGenericType && _dependencyDescriptions.All(t => t.DependencyType != dependency))
        {
            return ResolveGenericDependencies(dependency, option);
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        if (dependencyDescription is null) throw new DependencyNotFoundException(dependency);
        var relations = GetRelations(dependencyDescription, option?.IncludeDisabled is true);
        List<object> results = new();
        foreach (var relation in relations)
        {
            var result = ResolveRelation(dependencyDescription, relation, option);
            results.Add(result);
        }

        return results;
    }

    public object ResolveDependency(Type dependency, DependencyResolveOption? option = null)
    {
        if (dependency.IsGenericType)
        {
            if (dependency.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                // check whether is IEnumerable
                // and then return the fully Implemented stuff
                var cachedGenericType = dependency.GenericTypeArguments[0];
                var resolves = ResolveDependencies(cachedGenericType, option);
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
                    return ResolveGenericDependency(dependency, option);
                }
            }
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        if (dependencyDescription is null) throw new DependencyNotFoundException(dependency);
        var relation = GetRelation(dependencyDescription, option?.IncludeDisabled is true);
        return ResolveRelation(dependencyDescription, relation, option);
    }

    public void ChangeResolveTarget(Type dependency, object? target)
    {
        var description = GetDependencyDescription(dependency);
        if (description is null) throw new DependencyNotFoundException(dependency);
        if (description.Lifetime == DependencyLifetime.Singleton)
        {
            _rootScope.SetImplementation(dependency, target);
        }

        if (_option.AutoNotifyDependencyChange)
            NotifyDependencyChange(description);
    }

    private object? ResolveGenericDependency(Type dependency, DependencyResolveOption? option)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        if (dependencyDescription is null) throw new DependencyNotFoundException(dependency);
        var relation = GetRelation(dependencyDescription, option?.IncludeDisabled is true);
        if (relation is null) return null;
        if (relation.DefaultImplementation is not null) return relation.DefaultImplementation;
        var implementType = relation.ImplementType;
        if (!dependency.ContainsGenericParameters)
            implementType = relation.ImplementType.MakeGenericType(dependency.GenericTypeArguments);
        return dependencyDescription.Lifetime switch
        {
            DependencyLifetime.Singleton => ResolveSingleton(implementType, option),
            DependencyLifetime.Transient => ResolveTransient(implementType, option),
            DependencyLifetime.Scoped => ResolveScoped(implementType, option),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private List<object> ResolveGenericDependencies(Type dependency, DependencyResolveOption? option)
    {
        var genericType = dependency.GetGenericTypeDefinition();
        var dependencyDescription = GetDependencyDescription(genericType);
        if (dependencyDescription is null) throw new DependencyNotFoundException(dependency);
        var relations = GetRelations(dependencyDescription, option?.IncludeDisabled is true);
        List<object> results = new();
        foreach (var relation in relations)
        {
            if (relation.DefaultImplementation is not null) results.Add(relation.DefaultImplementation);
            var implementType = relation.ImplementType;
            if (!dependency.ContainsGenericParameters)
                implementType = relation.ImplementType.MakeGenericType(dependency.GenericTypeArguments);
            var impl = dependencyDescription.Lifetime switch
            {
                DependencyLifetime.Singleton => ResolveSingleton(implementType, option),
                DependencyLifetime.Transient => ResolveTransient(implementType, option),
                DependencyLifetime.Scoped => ResolveScoped(implementType, option),
                _ => throw new ArgumentOutOfRangeException()
            };
            results.Add(impl);
        }

        return results;
    }

    private object ImplementActivator(Type implementType, DependencyResolveOption? option)
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
                parameters.Add(ResolveDependency(parameterInfo.ParameterType, option));
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

        if (option?.CheckAsyncConstructor is not false &&
            dependencyImpl is IAsyncConstructService asyncConstructService)
            asyncConstructService.InitializeService().ConfigureAwait(false);
        return dependencyImpl;
    }


    private object ResolveRelation(
        DependencyDescription dependencyDescription,
        DependencyRelation relation,
        DependencyResolveOption? option = null)
    {
        if (relation.DefaultImplementation is not null) return relation.DefaultImplementation;
        return dependencyDescription.Lifetime switch
        {
            DependencyLifetime.Singleton => ResolveSingleton(relation.ImplementType, option),
            DependencyLifetime.Transient => ResolveTransient(relation.ImplementType, option),
            DependencyLifetime.Scoped => ResolveScoped(relation.ImplementType, option),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private object ResolveScoped(Type implementType, DependencyResolveOption? option)
    {
        if (option?.Scope is null) throw new ScopeNotSetException();
        if (option.Scope.Exist(implementType))
        {
            var ret = option.Scope.GetImplement(implementType);
            if (ret is not null) return ret;
        }

        var impl = ImplementActivator(implementType, option);
        option.Scope.SetImplementation(implementType, impl);
        return impl;
    }

    private object ResolveTransient(Type implementType, DependencyResolveOption? option)
    {
        var impl = ImplementActivator(implementType, option);
        return impl;
    }

    private object ResolveSingleton(Type implementType, DependencyResolveOption? option)
    {
        if (_rootScope.Exist(implementType))
        {
            var ret = _rootScope.GetImplement(implementType);
            if (ret is not null) return ret;
        }

        var impl = ImplementActivator(implementType, option);
        _rootScope.SetImplementation(implementType, impl);
        return impl;
    }
}