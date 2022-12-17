using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Extensions;

public static class AddDependencyExtension
{
    public static async Task AddSingletonAsync<TDependency, TImplement>(this IDepository depository) where TImplement : TDependency
    {
        await AddAsync<TDependency, TImplement>(depository, DependencyLifetime.Singleton);
    }
    public static async Task AddScopedAsync<TDependency, TImplement>(this IDepository depository) where TImplement : TDependency
    {
        await AddAsync<TDependency, TImplement>(depository, DependencyLifetime.Scoped);
    }
    
    public static async Task AddTransientAsync<TDependency, TImplement>(this IDepository depository) where TImplement : TDependency
    {
        await AddAsync<TDependency, TImplement>(depository, DependencyLifetime.Transient);
    }

    private static async Task AddAsync<TDependency, TImplement>(IDepository depository, DependencyLifetime lifetime) where TImplement : TDependency
    {
        var dependencyDescription = await depository.GetDependencyAsync(typeof(TDependency));

        if (dependencyDescription is null)
        {
            dependencyDescription = new DependencyDescription
            {
                DependencyType = typeof(TDependency),
                ResolvePolicy = DependencyResolvePolicy.LastWin,
                Lifetime = lifetime
            };
            await depository.AddDependencyAsync(dependencyDescription);
        }

        await depository.AddRelationAsync(dependencyDescription, new DependencyRelation
        {
            ImplementType = typeof(TImplement),
        });
    }
}