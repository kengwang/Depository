using Depository.Abstraction.Models;

namespace Depository.Abstraction.Interfaces;

public interface IRelationDepository
{
    public Task AddRelationAsync(DependencyDescription dependency, DependencyRelation relation);

    /// <summary>
    /// Replace Dependency
    /// </summary>
    /// <param name="dependencyType"></param>
    /// <param name="relation"></param>
    /// <returns></returns>
    public Task ChangeFocusingRelationAsync(DependencyDescription dependencyType, DependencyRelation relation);

    /// <summary>
    /// Delete Relation
    /// </summary>
    /// <param name="dependencyType"></param>
    /// <param name="relation"></param>
    /// <returns></returns>
    public Task DeleteRelationAsync(DependencyDescription dependencyType, DependencyRelation relation);


    public Task ClearRelationsAsync(DependencyDescription dependencyType);
}