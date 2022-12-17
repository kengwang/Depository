using Depository.Abstraction.Interfaces;

namespace Depository.Extensions;

public static class ChangeRelationExtension
{
    public static async Task ChangeFocusingRelation<TDependency, TImplement>(this IDepository depository)
    {
        var depDes = await depository.GetDependencyAsync(typeof(TDependency));
        var relations = await depository.GetRelationsAsync(depDes!);
        await depository.ChangeFocusingRelationAsync(depDes!,
            relations.First(relation => relation.ImplementType == typeof(TImplement)));
    }
}