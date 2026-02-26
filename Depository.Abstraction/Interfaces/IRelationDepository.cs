using System.Diagnostics.CodeAnalysis;
using Depository.Abstraction.Models;

namespace Depository.Abstraction.Interfaces;

public interface IRelationDepository
{
    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public void AddRelation(DependencyDescription dependency, DependencyRelation relation);

    public DependencyRelation? GetRelation(DependencyDescription description, bool includeDisabled = false, string? relationName = null);
    public List<DependencyRelation> GetRelations(DependencyDescription description, bool includeDisabled = false);
    
    /// <summary>
    /// Replace Dependency
    /// </summary>
    /// <param name="description"></param>
    /// <param name="relation"></param>
    /// <returns></returns>
    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public void ChangeFocusingRelation(DependencyDescription description, DependencyRelation relation);

    /// <summary>
    /// Delete Relation
    /// </summary>
    /// <param name="description"></param>
    /// <param name="relation"></param>
    /// <returns></returns>
    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public void DeleteRelation(DependencyDescription description, DependencyRelation relation);


    public void ClearRelations(DependencyDescription description);

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public void DisableRelation(DependencyDescription description, DependencyRelation relation);

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public void EnableRelation(DependencyDescription description, DependencyRelation relation);
}