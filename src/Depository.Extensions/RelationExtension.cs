using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Exceptions;

namespace Depository.Extensions;

public static class RelationExtension
{
    public static void ChangeFocusingRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        if (depDes is null) throw new DependencyNotFoundException(typeof(TDependency));
        var relations = depository.GetRelations(depDes!);
        depository.ChangeFocusingRelation(depDes!,
            relations.First(relation => relation.ImplementType == typeof(TImplement)));
    }

    public static void RemoveRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        if (depDes is null) throw new DependencyNotFoundException(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.ImplementType == typeof(TImplement));
        if (relation is null) throw new DependencyNotFoundException(typeof(TDependency));
        depository.DeleteRelation(depDes!, relation!);
    }
    
    public static void DisableRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        if (depDes is null) throw new DependencyNotFoundException(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.ImplementType == typeof(TImplement));
        if (relation is null) throw new DependencyNotFoundException(typeof(TDependency));
        depository.DisableRelation(depDes!, relation!);
    }
    
    public static void DisableRelation<TDependency>(this IDepository depository, string relationName)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        if (depDes is null) throw new DependencyNotFoundException(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.Name == relationName);
        if (relation is null) throw new DependencyNotFoundException(typeof(TDependency));
        depository.DisableRelation(depDes!, relation!);
    }
    
    public static void EnableRelation<TDependency>(this IDepository depository, string relationName)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        if (depDes is null) throw new DependencyNotFoundException(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.Name == relationName);
        if (relation is null) throw new DependencyNotFoundException(typeof(TDependency));
        depository.EnableRelation(depDes!, relation!);
    }
    
    public static void EnableRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        if (depDes is null) throw new DependencyNotFoundException(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.ImplementType == typeof(TImplement));
        if (relation is null) throw new DependencyNotFoundException(typeof(TDependency));
        depository.EnableRelation(depDes!, relation!);
    }
}
