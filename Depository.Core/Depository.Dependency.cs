using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    private readonly HashSet<DependencyDescription> _dependencyDescriptions = new();

    public void AddDependency(DependencyDescription description)
    {
        _dependencyDescriptions.RemoveWhere(t => t.DependencyType == description.DependencyType);
        _dependencyDescriptions.Add(description);
    }


    public bool DependencyExist(Type dependencyType)
    {
        return _dependencyDescriptions.Any(des => des.DependencyType == dependencyType);
    }

    public DependencyDescription? GetDependency(Type dependencyType)
    {
        return _dependencyDescriptions.FirstOrDefault(dep => dep.DependencyType == dependencyType) ??
                               null;
    }

    public void DeleteDependency(DependencyDescription description)
    {
        _dependencyRelations.Remove(description);
        _dependencyDescriptions.Remove(description);
    }

    public void ClearAllDependencies()
    {
        _dependencyDescriptions.Clear();
        _dependencyRelations.Clear();
    }
    
    private DependencyDescription? GetDependencyDescription(Type dependency)
    {
        var dependencyDescription = _dependencyDescriptions.FirstOrDefault(t => t.DependencyType == dependency);
        return dependencyDescription;
    }
}