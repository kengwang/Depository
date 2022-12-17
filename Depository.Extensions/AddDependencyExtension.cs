using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Extensions;

public static class AddDependencyExtension
{
    public static async Task AddSingletonAsync<TDependency, TImplement>(this IDepository depository) where TImplement : TDependency
    {
        var dependencyDescription = await depository.GetDependencyAsync(typeof(TDependency));

        if (dependencyDescription is null)
        {
            dependencyDescription = new DependencyDescription
            {
                DependencyType = typeof(TDependency),
                ResolvePolicy = DependencyResolvePolicy.LastWin,
                Lifetime = DependencyLifetime.Singleton
            };
            await depository.AddDependencyAsync(dependencyDescription);
        }

        await depository.AddRelationAsync(dependencyDescription, new DependencyRelation
        {
            ImplementType = typeof(TImplement),
        });
    }

    public static async Task<T> Resolve<T>(this IDepositoryResolve depository)
    {
        return (T)await depository.ResolveDependencyAsync(typeof(T));
    }
}