using Depository.Abstraction.Models;

namespace Depository.Abstraction.Interfaces;

public interface IDepositoryDependency
{
    /// <summary>
    /// Add dependency description
    /// </summary>
    /// <param name="description">Dependency Description</param>
    /// <returns></returns>
    public void AddDependency(DependencyDescription description);

    /// <summary>
    /// Check if dependency exist
    /// </summary>
    /// <param name="dependencyType"></param>
    /// <returns></returns>
    public bool DependencyExist(Type dependencyType);

    /// <summary>
    /// Get Dependency
    /// </summary>
    /// <param name="dependencyType">Dependency Type</param>
    /// <returns>Dependency Description, null for not found</returns>
    public DependencyDescription? GetDependency(Type dependencyType);

    /// <summary>
    /// Delete Dependency
    /// </summary>
    /// <param name="description">Dependency Description</param>
    /// <returns></returns>
    public void DeleteDependency(DependencyDescription description);
    
    /// <summary>
    /// Set Dependency Decoration
    /// </summary>
    /// <param name="description"></param>
    /// <param name="decorationRelation"></param>
    public void SetDependencyDecoration(DependencyDescription description, DependencyRelation? decorationRelation);
    
    /// <summary>
    /// Clear all dependencies
    /// </summary>
    public void ClearAllDependencies();
}