using Depository.Abstraction.Models;

namespace Depository.Abstraction.Interfaces;

public interface IRelationDepository
{
    public Task AddRelationAsync(DependencyDescription dependency, DependencyRelation relation);

    public Task<DependencyRelation> GetRelationAsync(DependencyDescription dependency);
    public Task<List<DependencyRelation>> GetRelationsAsync(DependencyDescription dependency);
    
    /// <summary>
    /// Replace Dependency
    /// </summary>
    /// <param name="dependencyDescription"></param>
    /// <param name="relation"></param>
    /// <returns></returns>
    public Task ChangeFocusingRelationAsync(DependencyDescription dependencyDescription, DependencyRelation relation);

    /// <summary>
    /// Delete Relation
    /// </summary>
    /// <param name="dependencyType"></param>
    /// <param name="relation"></param>
    /// <returns></returns>
    public Task DeleteRelationAsync(DependencyDescription dependencyType, DependencyRelation relation);


    public Task ClearRelationsAsync(DependencyDescription dependencyType);
}