using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;

namespace Depository.Core;

public partial class Depository
{
    private readonly Dictionary<DependencyDescription, HashSet<DependencyRelation>> _dependencyRelations = new();
    private readonly Dictionary<DependencyDescription, DependencyRelation> _currentFocusing = new();

    public void AddRelation(DependencyDescription dependency, DependencyRelation relation)
    {
        if (Option.CheckerOption.ImplementIsInheritedFromDependency &&
            dependency.DependencyType.IsAssignableFrom(relation.ImplementType))
            throw new ImplementNotInheritedFromDependencyException();
        if (Option.CheckerOption.ImplementIsInstantiable &&
            (relation.ImplementType.IsAbstract || relation.ImplementType.IsInterface))
            throw new ImplementNotInstantiableException();


        if (!_dependencyRelations.TryGetValue(dependency, out var relations))
        {
            relations = new HashSet<DependencyRelation>();
            _dependencyRelations.Add(dependency, relations);
        }

        if (Option.CheckerOption.CheckImplementationDuplication)
            switch (Option.ImplementTypeDuplicatedAction)
            {
                case ImplementTypeDuplicatedAction.Throw:
                    if (relations.Any(dependencyRelation => dependencyRelation == relation))
                        throw new ImplementDuplicatedException();
                    break;
                case ImplementTypeDuplicatedAction.Ignore:
                    if (relations.Any(dependencyRelation => dependencyRelation == relation))
                        return;
                    break;
            }

        relations.Add(relation);


        // 通知修改
        if (Option.AutoNotifyDependencyChange)
            NotifyDependencyChange(dependency, 1);
    }

    public void DeleteRelation(DependencyDescription dependencyType, DependencyRelation relation)
    {
        if (_dependencyRelations.TryGetValue(dependencyType, out var relations))
        {
            relations.Remove(relation);
        }

        if (Option.AutoNotifyDependencyChange)
            NotifyDependencyChange(dependencyType);
    }

    public void ClearRelations(DependencyDescription dependencyType)
    {
        _dependencyRelations.Remove(dependencyType);
    }

    public void DisableRelation(DependencyDescription description, DependencyRelation relation)
    {
        relation.IsEnabled = false;
        if (Option.AutoNotifyDependencyChange)
            NotifyDependencyChange(description);
    }

    public void EnableRelation(DependencyDescription description, DependencyRelation relation)
    {
        relation.IsEnabled = true;
        if (Option.AutoNotifyDependencyChange)
            NotifyDependencyChange(description);
    }


    public void ChangeFocusingRelation(DependencyDescription dependencyDescription,
        DependencyRelation relation)
    {
        _currentFocusing[dependencyDescription] = relation;
        if (Option.AutoNotifyDependencyChange)
            NotifyDependencyChange(dependencyDescription, 2);
    }

    public DependencyRelation? GetRelation(DependencyDescription dependencyDescription,
        bool includeDisabled = false, string? relationName = null)
    {
        if (_currentFocusing.TryGetValue(dependencyDescription, out var relation) && relation.IsEnabled)
            return relation;
        if (_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
        {
            if (relations.Count == 0) throw new RelationNotFoundException();
            if (relationName is not null)
            {
                var resolvedRelation = relations.FirstOrDefault(t => t.Name == relationName);
                if (resolvedRelation is null)
                    throw new DependencyNotFoundException(dependencyDescription.DependencyType);
                return resolvedRelation;
            }

            relation = includeDisabled ? relations.Last() : relations.LastOrDefault(t => t.IsEnabled);
            if (!includeDisabled)
                _currentFocusing[dependencyDescription] = relation;
        }
        else
        {
            throw new RelationNotFoundException();
        }

        return relation;
    }

    public List<DependencyRelation> GetRelations(DependencyDescription dependencyDescription,
        bool includeDisabled = false)
    {
        if (_dependencyRelations.TryGetValue(dependencyDescription, out var relations))
            return
                includeDisabled
                    ? relations.ToList()
                    : relations.Where(relation => relation.IsEnabled).ToList();

        return new List<DependencyRelation>();
    }
}