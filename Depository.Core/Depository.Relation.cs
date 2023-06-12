using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public partial class Depository
{
    private readonly Dictionary<DependencyDescription, HashSet<DependencyRelation>> _dependencyRelations = new();
    private readonly Dictionary<DependencyDescription, DependencyRelation> _currentFocusing = new();

    public async Task AddRelationAsync(DependencyDescription dependency, DependencyRelation relation)
    {
        if (_option.CheckerOption.ImplementIsInheritedFromDependency &&
            dependency.DependencyType.IsAssignableFrom(relation.ImplementType))
            throw new ImplementNotInheritedFromDependencyException();
        if (_option.CheckerOption.ImplementIsInstantiable &&
            (relation.ImplementType.IsAbstract || relation.ImplementType.IsInterface))
            throw new ImplementNotInstantiableException();


        if (!_dependencyRelations.TryGetValue(dependency, out var relations))
        {
            relations = new HashSet<DependencyRelation>();
            _dependencyRelations.Add(dependency, relations);
        }

        switch (_option.ImplementTypeDuplicatedAction)
        {
            case ImplementTypeDuplicatedAction.Throw:
                if (relations.Any(dependencyRelation => dependencyRelation.ImplementType == relation.ImplementType))
                    throw new ImplementDuplicatedException();
                break;
            case ImplementTypeDuplicatedAction.Ignore:
                if (relations.Any(dependencyRelation => dependencyRelation.ImplementType == relation.ImplementType))
                    return;
                break;
        }

        relations.Add(relation);


        // 通知修改
        if (_option.AutoNotifyDependencyChange)
            await NotifyDependencyChange(dependency, 1);
    }

    public async Task DeleteRelationAsync(DependencyDescription dependencyType, DependencyRelation relation)
    {
        if (_dependencyRelations.TryGetValue(dependencyType, out var relations))
        {
            relations.Remove(relation);
        }

        if (_option.AutoNotifyDependencyChange)
            await NotifyDependencyChange(dependencyType);
    }

    public Task ClearRelationsAsync(DependencyDescription dependencyType)
    {
        _dependencyRelations.Remove(dependencyType);
        return Task.CompletedTask;
    }

    public async Task DisableRelationAsync(DependencyDescription description, DependencyRelation relation)
    {
        relation.IsEnabled = false;
        if (_option.AutoNotifyDependencyChange)
            await NotifyDependencyChange(description);
    }

    public async Task EnableRelationAsync(DependencyDescription description, DependencyRelation relation)
    {
        relation.IsEnabled = true;
        if (_option.AutoNotifyDependencyChange)
            await NotifyDependencyChange(description);
    }


    public async Task ChangeFocusingRelationAsync(DependencyDescription dependencyDescription,
        DependencyRelation relation)
    {
        _currentFocusing[dependencyDescription] = relation;
        if (_option.AutoNotifyDependencyChange)
            await NotifyDependencyChange(dependencyDescription, 2);
    }

    public Task<DependencyRelation> GetRelationAsync(DependencyDescription dependencyDescription,
        bool includeDisabled = false, string? relationName = null)
    {
        if (_currentFocusing.TryGetValue(dependencyDescription, out var relation) && relation.IsEnabled)
            return Task.FromResult(relation);
        if (_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
        {
            if (relations.Count == 0) throw new RelationNotFoundException();
            if (relationName is not null)
            {
                var resolvedRelation = relations.FirstOrDefault(t => t.Name == relationName);
                if (resolvedRelation is null)
                    throw new DependencyNotFoundException(dependencyDescription.DependencyType);
                return Task.FromResult(resolvedRelation);
            }

            relation = includeDisabled ? relations.Last() : relations.LastOrDefault(t => t.IsEnabled);
            if (!includeDisabled)
                _currentFocusing[dependencyDescription] = relation;
        }
        else
        {
            throw new RelationNotFoundException();
        }

        return Task.FromResult(relation);
    }

    public Task<List<DependencyRelation>> GetRelationsAsync(DependencyDescription dependencyDescription,
        bool includeDisabled = false)
    {
        if (_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
            return Task.FromResult(
                includeDisabled
                    ? relations.ToList()
                    : relations.Where(relation => relation.IsEnabled).ToList()
            );

        return Task.FromResult(new List<DependencyRelation>());
    }
}