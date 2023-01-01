using Depository.Abstraction.Models;

namespace Depository.Abstraction.Interfaces;

public interface IDependencyDepository
{
    /// <summary>
    /// Add dependency description
    /// </summary>
    /// <param name="description">Dependency Description</param>
    /// <returns></returns>
    public Task AddDependencyAsync(DependencyDescription description);

    public Task<bool> DependencyExist(Type dependencyType);

    /// <summary>
    /// Get Dependency
    /// </summary>
    /// <param name="dependencyType"></param>
    /// <returns></returns>
    public Task<DependencyDescription?> GetDependencyAsync(Type dependencyType);

    /// <summary>
    /// Delete Dependency
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public Task DeleteDependencyAsync(DependencyDescription description);
    
    public Task ClearAllDependenciesAsync();
}