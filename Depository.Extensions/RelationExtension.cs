using Depository.Abstraction.Interfaces;

namespace Depository.Extensions;

public static class RelationExtension
{
    public static async Task ChangeFocusingRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = await depository.GetDependencyAsync(typeof(TDependency));
        var relations = await depository.GetRelationsAsync(depDes!);
        await depository.ChangeFocusingRelationAsync(depDes!,
            relations.First(relation => relation.ImplementType == typeof(TImplement)));
    }

    public static async Task RemoveRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = await depository.GetDependencyAsync(typeof(TDependency));
        var relation =
            (await depository.GetRelationsAsync(depDes!, true)).FirstOrDefault(rel =>
                rel.ImplementType == typeof(TImplement));
        await depository.DeleteRelationAsync(depDes!, relation!);
    }
    
    public static async Task DisableRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = await depository.GetDependencyAsync(typeof(TDependency));
        var relation =
            (await depository.GetRelationsAsync(depDes!, true)).FirstOrDefault(rel =>
                rel.ImplementType == typeof(TImplement));
        await depository.DisableRelationAsync(depDes!, relation!);
    }
    
    public static async Task DisableRelation<TDependency>(this IDepository depository, string relationName)
    {
        var depDes = await depository.GetDependencyAsync(typeof(TDependency));
        var relation =
            (await depository.GetRelationsAsync(depDes!, true)).FirstOrDefault(rel =>
                rel.Name == relationName);
        await depository.DisableRelationAsync(depDes!, relation!);
    }
    
    public static async Task EnableRelation<TDependency>(this IDepository depository, string relationName)
    {
        var depDes = await depository.GetDependencyAsync(typeof(TDependency));
        var relation =
            (await depository.GetRelationsAsync(depDes!, true)).FirstOrDefault(rel =>
                rel.Name == relationName);
        await depository.EnableRelationAsync(depDes!, relation!);
    }
    
    public static async Task EnableRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = await depository.GetDependencyAsync(typeof(TDependency));
        var relation =
            (await depository.GetRelationsAsync(depDes!, true)).FirstOrDefault(rel =>
                rel.ImplementType == typeof(TImplement));
        await depository.EnableRelationAsync(depDes!, relation!);
    }
}