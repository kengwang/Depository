using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;

namespace Depository.Core;

public partial class Depository
{
    private readonly List<DependencyDescription> _dependencyDescriptions = new();

    public Task AddDependencyAsync(DependencyDescription description)
    {
        _dependencyDescriptions.RemoveAll(t => t.DependencyType == description.DependencyType);
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
    
    private DependencyDescription GetDependencyDescription(Type dependency)
    {
        var dependencyDescription = _dependencyDescriptions.FirstOrDefault(t => t.DependencyType == dependency);
        if (dependencyDescription is null) throw new DependencyNotFoundException { DependencyType = dependency };
        return dependencyDescription;
    }
}