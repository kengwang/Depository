using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Extensions;

public static class AddDependencyExtension
{
    public static async Task AddSingletonAsync<TDependency, TImplement>(this IDepository depository,
        object? defaultImplement = null, string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        await AddAsync<TDependency, TImplement>(depository, DependencyLifetime.Singleton, defaultImplement,
            relationName, isEnabled);

    public static async Task
        AddSingletonAsync<TService>(this IDepository depository, object? defaultImplement = null,
            string? relationName = null, bool isEnabled = true) =>
        await AddAsync<TService>(depository, DependencyLifetime.Singleton, defaultImplement, relationName, isEnabled);

    public static async Task AddScopedAsync<TDependency, TImplement>(this IDepository depository,
        string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        await AddAsync<TDependency, TImplement>(depository, DependencyLifetime.Scoped, null, relationName, isEnabled);

    public static async Task AddScopedAsync<TService>(this IDepository depository, string? relationName = null,
        bool isEnabled = true) =>
        await AddAsync<TService>(depository, DependencyLifetime.Scoped, null, relationName, isEnabled);

    public static async Task AddTransientAsync<TDependency, TImplement>(this IDepository depository,
        string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        await AddAsync<TDependency, TImplement>(depository, DependencyLifetime.Transient, null, relationName,
            isEnabled);

    public static async Task AddTransientAsync<TService>(this IDepository depository, string? relationName = null,
        bool isEnabled = true) =>
        await AddAsync<TService>(depository, DependencyLifetime.Transient, null, relationName, isEnabled);

    public static async Task AddAsync<TDependency, TImplement>(this IDepository depository, DependencyLifetime lifetime,
        object? defaultImplement = null, string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        await AddAsync(depository, typeof(TDependency), typeof(TImplement), lifetime, defaultImplement, relationName,
            isEnabled);

    public static async Task AddAsync<TService>(this IDepository depository, DependencyLifetime lifetime,
        object? defaultImplement = null, string? relationName = null, bool isEnabled = true) =>
        await AddAsync(depository, typeof(TService), typeof(TService), lifetime, defaultImplement, relationName,
            isEnabled);

    public static async Task AddSingletonAsync(this IDepository depository, Type dependencyType, Type implementType,
        object? defaultImplement = null, string? relationName = null, bool isEnabled = true)
        => await AddAsync(depository, dependencyType, implementType, DependencyLifetime.Singleton, defaultImplement,
            relationName, isEnabled);

    public static async Task AddTransientAsync(this IDepository depository, Type dependencyType, Type implementType,
        string? relationName = null, bool isEnabled = true)
        => await AddAsync(depository, dependencyType, implementType, DependencyLifetime.Transient, null, relationName,
            isEnabled);

    public static async Task AddScopedAsync(this IDepository depository, Type dependencyType, Type implementType,
        string? relationName = null, bool isEnabled = true)
        => await AddAsync(depository, dependencyType, implementType, DependencyLifetime.Scoped, null, relationName,
            isEnabled);

    public static async Task AddAsync(this IDepository depository, Type dependencyType, Type implementType,
        DependencyLifetime lifetime, object? defaultImplement = null, string? relationName = null,
        bool isEnabled = true)
    {
        var dependencyDescription = await depository.GetDependencyAsync(dependencyType);

        if (dependencyDescription is null)
        {
            dependencyDescription = new DependencyDescription(DependencyType: dependencyType,
                ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: lifetime);
            await depository.AddDependencyAsync(dependencyDescription);
        }

        await depository.AddRelationAsync(dependencyDescription,
            new DependencyRelation(RelationType: DependencyRelationType.Once, ImplementType: implementType,
                DefaultImplementation: defaultImplement, Name: relationName, IsEnabled: isEnabled));
    }
}