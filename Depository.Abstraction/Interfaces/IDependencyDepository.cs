using Depository.Abstraction.Models;

namespace Depository.Abstraction.Interfaces;

public interface IDependencyDepository
{
    /// <summary>
    /// Add dependency description
    /// </summary>
    /// <param name="description">Dependency Description</param>
    /// <returns></returns>
    public void AddDependency(DependencyDescription description);

    public bool DependencyExist(Type dependencyType);

    /// <summary>
    /// Get Dependency
    /// </summary>
    /// <param name="dependencyType"></param>
    /// <returns></returns>
    public DependencyDescription? GetDependency(Type dependencyType);

    /// <summary>
    /// Delete Dependency
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public void DeleteDependency(DependencyDescription description);
    
    public void ClearAllDependencies();
}