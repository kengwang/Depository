using System.Diagnostics.CodeAnalysis;
using Depository.Abstraction.Interfaces;

namespace Depository.Extensions;

public static class RelationExtension
{
    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public static void ChangeFocusingRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        var relations = depository.GetRelations(depDes!);
        depository.ChangeFocusingRelation(depDes!,
            relations.First(relation => relation.ImplementType == typeof(TImplement)));
    }

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public static void RemoveRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.ImplementType == typeof(TImplement));
        depository.DeleteRelation(depDes!, relation!);
    }

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public static void DisableRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.ImplementType == typeof(TImplement));
        depository.DisableRelation(depDes!, relation!);
    }

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public static void DisableRelation<TDependency>(this IDepository depository, string relationName)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.Name == relationName);
        depository.DisableRelation(depDes!, relation!);
    }

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public static void EnableRelation<TDependency>(this IDepository depository, string relationName)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.Name == relationName);
        depository.EnableRelation(depDes!, relation!);
    }

    [RequiresDynamicCode("Dispatching dependency-change notifications uses MakeGenericType at runtime.")]
    public static void EnableRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = depository.GetDependency(typeof(TDependency));
        var relation =
            (depository.GetRelations(depDes!, true)).FirstOrDefault(rel =>
                rel.ImplementType == typeof(TImplement));
        depository.EnableRelation(depDes!, relation!);
    }
}