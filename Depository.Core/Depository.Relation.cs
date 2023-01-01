using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    private readonly Dictionary<DependencyDescription, List<DependencyRelation>> _dependencyRelations = new();
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
            relations = new List<DependencyRelation>();
            _dependencyRelations.Add(dependency, relations);
        }

        relations.Add(relation);

        // 通知修改
        if (_option.AutoNotifyDependencyChange)
            await NotifyDependencyChange(dependency);
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


    public async Task ChangeFocusingRelationAsync(DependencyDescription dependencyDescription,
        DependencyRelation relation)
    {
        _currentFocusing[dependencyDescription] = relation;
        if (_option.AutoNotifyDependencyChange)
            await NotifyDependencyChange(dependencyDescription);
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
}