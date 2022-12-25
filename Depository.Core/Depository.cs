using System.Collections;
using System.Runtime.CompilerServices;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Core;

public class Depository : IDepository
{
    private readonly DepositoryOption _option = new();
    private readonly Dictionary<DependencyDescription, List<DependencyRelation>> _dependencyRelations = new();
    private readonly Dictionary<DependencyDescription, DependencyRelation> _currentFocusing = new();
    private readonly List<DependencyDescription> _dependencyDescriptions = new();
    private readonly ConditionalWeakTable<object, List<Type>> _resolvedTypes = new();
    private readonly Dictionary<Type, List<WeakReference>> _usedImpls = new();
    private readonly DepositoryResolveScope _rootScope = new();

    public Task AddDependencyAsync(DependencyDescription description)
    {
        _dependencyDescriptions.Add(description);
        return Task.CompletedTask;
    }

    public Task<bool> DependencyExist(Type dependencyType)
    {
        return Task.FromResult(_dependencyDescriptions.Any(des => des.DependencyType == dependencyType));
    }

    public Task<DependencyDescription?> GetDependencyAsync(Type dependencyType)
    {
        return Task.FromResult(_dependencyDescriptions.FirstOrDefault(dep => dep.DependencyType == dependencyType) ??
                               null);
    }

    public Task DeleteDependencyAsync(DependencyDescription dependencyDescription)
    {
        _dependencyDescriptions.Remove(dependencyDescription);
        return Task.CompletedTask;
    }

    public Task ClearAllDependenciesAsync()
    {
        _dependencyDescriptions.Clear();
        return Task.CompletedTask;
    }

    public Task AddRelationAsync(DependencyDescription dependency, DependencyRelation relation)
    {
        if (!_dependencyRelations.TryGetValue(dependency, out var relations))
        {
            relations = new List<DependencyRelation>();
            _dependencyRelations.Add(dependency, relations);
        }

        relations.Add(relation);

        // 通知修改
        if (_option.AutoNotifyDependencyChange)
        {
        }

        return Task.CompletedTask;
    }

    public async Task ChangeFocusingRelationAsync(DependencyDescription dependencyDescription,
        DependencyRelation relation)
    {
        _currentFocusing[dependencyDescription] = relation;
        if (_option.AutoNotifyDependencyChange)
        {
            // 回溯使用了此 Relation 的东西
            // TODO: 检查此处逻辑是否会造成复用
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
    }

    public Task DeleteRelationAsync(DependencyDescription dependencyType, DependencyRelation relation)
    {
        if (_dependencyRelations.TryGetValue(dependencyType, out var relations))
        {
            relations.Remove(relation);
        }

        return Task.CompletedTask;
    }

    public Task ClearRelationsAsync(DependencyDescription dependencyType)
    {
        _dependencyRelations.Remove(dependencyType);
        return Task.CompletedTask;
    }


    private DependencyDescription GetDependencyDescription(Type dependency)
    {
        var dependencyDescription = _dependencyDescriptions.FirstOrDefault(t => t.DependencyType == dependency);
        if (dependencyDescription is null) throw new DependencyNotFoundException { DependencyType = dependency };
        return dependencyDescription;
    }


    public async Task<List<object>> ResolveDependenciesAsync(Type dependency, DependencyResolveOption? option)
    {
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
        if (dependency.IsGenericType && dependency.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            // check whether is IEnumerable
            // and then return the fully Implemented stuff
            var cachedGenericType = dependency.GenericTypeArguments[0];
            var resolves = (await ResolveDependenciesAsync(cachedGenericType, option));
            var impls = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(cachedGenericType));
            foreach (var impl in resolves)
            {
                if (impl is null) continue;
                if (cachedGenericType.IsInstanceOfType(impl))
                    impls.Add(impl);
            }

            return impls;
        }

        var dependencyDescription = GetDependencyDescription(dependency);
        var relation = await GetRelationAsync(dependencyDescription);
        return await ResolveRelationAsync(dependencyDescription, relation, option);
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

    public Task<DependencyRelation> GetRelationAsync(DependencyDescription dependencyDescription)
    {
        if (_currentFocusing.TryGetValue(dependencyDescription, out var relation)) return Task.FromResult(relation);
        if (_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
        {
            if (relations.Count == 0) throw new RelationNotFoundException();
            switch (dependencyDescription.ResolvePolicy)
            {
                case DependencyResolvePolicy.LastWin:
                    relation = relations.Last();
                    _currentFocusing[dependencyDescription] = relation;
                    break;
                case DependencyResolvePolicy.FirstWin:
                    relation = relations[0];
                    _currentFocusing[dependencyDescription] = relation;
                    break;
                case DependencyResolvePolicy.WaitForFocusing:
                    throw new RelationNotFocusingException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            throw new RelationNotFoundException();
        }

        return Task.FromResult(relation);
    }

    public Task<List<DependencyRelation>> GetRelationsAsync(DependencyDescription dependencyDescription)
    {
        if (_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
            return Task.FromResult(relations);

        throw new RelationNotFoundException();
    }


    public Task RunAsync(Type serviceType)
    {
        throw new NotImplementedException();
    }
}