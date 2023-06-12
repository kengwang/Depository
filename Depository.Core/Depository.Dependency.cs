using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    private readonly HashSet<DependencyDescription> _dependencyDescriptions = new();

    public Task AddDependencyAsync(DependencyDescription description)
    {
        _dependencyDescriptions.RemoveWhere(t => t.DependencyType == description.DependencyType);
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

    public Task DeleteDependencyAsync(DependencyDescription description)
    {
        _dependencyRelations.Remove(description);
        _dependencyDescriptions.Remove(description);
        return Task.CompletedTask;
    }

    public Task ClearAllDependenciesAsync()
    {
        _dependencyDescriptions.Clear();
        _dependencyRelations.Clear();
        return Task.CompletedTask;
    }
    
    private DependencyDescription? GetDependencyDescription(Type dependency)
    {
        var dependencyDescription = _dependencyDescriptions.FirstOrDefault(t => t.DependencyType == dependency);
        return dependencyDescription;
    }
}