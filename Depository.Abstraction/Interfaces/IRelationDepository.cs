using Depository.Abstraction.Models;

namespace Depository.Abstraction.Interfaces;

public interface IRelationDepository
{
    public Task AddRelationAsync(DependencyDescription dependency, DependencyRelation relation);

    public Task<DependencyRelation> GetRelationAsync(DependencyDescription description, bool includeDisabled = false, string? relationName = null);
    public Task<List<DependencyRelation>> GetRelationsAsync(DependencyDescription description, bool includeDisabled = false);
    
    /// <summary>
    /// Replace Dependency
    /// </summary>
    /// <param name="description"></param>
    /// <param name="relation"></param>
    /// <returns></returns>
    public Task ChangeFocusingRelationAsync(DependencyDescription description, DependencyRelation relation);

    /// <summary>
    /// Delete Relation
    /// </summary>
    /// <param name="description"></param>
    /// <param name="relation"></param>
    /// <returns></returns>
    public Task DeleteRelationAsync(DependencyDescription description, DependencyRelation relation);


    public Task ClearRelationsAsync(DependencyDescription description);

    public Task DisableRelationAsync(DependencyDescription description, DependencyRelation relation);
    
    public Task EnableRelationAsync(DependencyDescription description, DependencyRelation relation);
}