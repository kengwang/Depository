using System.Diagnostics.CodeAnalysis;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    private readonly HashSet<DependencyDescription> _dependencyDescriptions = new();
    private readonly Dictionary<Type, DependencyDescription> _dependencyDescriptionsByType = new();

    public void AddDependency(DependencyDescription description)
    {
        if (_dependencyDescriptionsByType.TryGetValue(description.DependencyType, out var existing))
        {
            _dependencyDescriptions.Remove(existing);
        }

        _dependencyDescriptionsByType[description.DependencyType] = description;
        _dependencyDescriptions.Add(description);
    }

    public bool DependencyExist(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependencyType)
    {
        if (_dependencyDescriptionsByType.ContainsKey(dependencyType)) return true;
        return dependencyType.IsGenericType &&
               _dependencyDescriptionsByType.ContainsKey(dependencyType.GetGenericTypeDefinition());
    }

    public DependencyDescription? GetDependency(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependencyType)
    {
        _dependencyDescriptionsByType.TryGetValue(dependencyType, out var dependency);
        return dependency;
    }

    public void DeleteDependency(DependencyDescription description)
    {
        _dependencyRelations.Remove(description);
        _dependencyDescriptions.Remove(description);
        _dependencyDescriptionsByType.Remove(description.DependencyType);
    }

    public void SetDependencyDecoration(DependencyDescription description, DependencyRelation? decorationRelation)
    {
        description.DecorationRelation = decorationRelation;
    }

    public void ClearAllDependencies()
    {
        _dependencyDescriptions.Clear();
        _dependencyDescriptionsByType.Clear();
        _dependencyRelations.Clear();
    }

    private DependencyDescription? GetDependencyDescription(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependency)
    {
        _dependencyDescriptionsByType.TryGetValue(dependency, out var dependencyDescription);
        return dependencyDescription;
    }
}
