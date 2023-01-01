using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Extensions;

public static class AddDependencyExtension
{
    public static async Task AddSingletonAsync<TDependency, TImplement>(this IDepository depository, object? defaultImplement = null)
        where TImplement : TDependency =>
        await AddAsync<TDependency, TImplement>(depository, DependencyLifetime.Singleton, defaultImplement);
    
    public static async Task AddSingletonAsync<TService>(this IDepository depository, object? defaultImplement = null) =>
        await AddAsync<TService>(depository, DependencyLifetime.Singleton, defaultImplement);

    public static async Task AddScopedAsync<TDependency, TImplement>(this IDepository depository)
        where TImplement : TDependency =>
        await AddAsync<TDependency, TImplement>(depository, DependencyLifetime.Scoped);
    
    public static async Task AddScopedAsync<TService>(this IDepository depository) =>
        await AddAsync<TService>(depository, DependencyLifetime.Scoped);

    public static async Task AddTransientAsync<TDependency, TImplement>(this IDepository depository)
        where TImplement : TDependency =>
        await AddAsync<TDependency, TImplement>(depository, DependencyLifetime.Transient);
    
    public static async Task AddTransientAsync<TService>(this IDepository depository) =>
        await AddAsync<TService>(depository, DependencyLifetime.Transient);

    public static async Task AddAsync<TDependency, TImplement>(this IDepository depository, DependencyLifetime lifetime, object? defaultImplement = null)
        where TImplement : TDependency =>
        await AddAsync(depository, typeof(TDependency), typeof(TImplement), lifetime, defaultImplement);
    
    public static async Task AddAsync<TService>(this IDepository depository, DependencyLifetime lifetime, object? defaultImplement = null) =>
        await AddAsync(depository, typeof(TService), typeof(TService), lifetime, defaultImplement);

    public static async Task AddSingletonAsync(this IDepository depository, Type dependencyType, Type implementType, object? defaultImplement = null)
        => await AddAsync(depository, dependencyType, implementType, DependencyLifetime.Singleton, defaultImplement);
    
    public static async Task AddTransientAsync(this IDepository depository, Type dependencyType, Type implementType)
        => await AddAsync(depository, dependencyType, implementType, DependencyLifetime.Transient);
    
    public static async Task AddScopedAsync(this IDepository depository, Type dependencyType, Type implementType)
        => await AddAsync(depository, dependencyType, implementType, DependencyLifetime.Scoped);
    
    public static async Task AddAsync(this IDepository depository, Type dependencyType, Type implementType,
        DependencyLifetime lifetime, object? defaultImplement = null)
    {
        var dependencyDescription = await depository.GetDependencyAsync(dependencyType);

        if (dependencyDescription is null)
        {
            dependencyDescription = new DependencyDescription
            {
                DependencyType = dependencyType,
                ResolvePolicy = DependencyResolvePolicy.LastWin,
                Lifetime = lifetime
            };
            await depository.AddDependencyAsync(dependencyDescription);
        }

        await depository.AddRelationAsync(dependencyDescription, new DependencyRelation
        {
            ImplementType = implementType,
            DefaultImplementation = defaultImplement
        });
    }
}