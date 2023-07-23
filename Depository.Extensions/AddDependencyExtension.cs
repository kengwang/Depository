using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Extensions;

public static class AddDependencyExtension
{
    public static void AddSingleton<TDependency, TImplement>(this IDepository depository,
        object? defaultImplement = null, string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        Add<TDependency, TImplement>(depository, DependencyLifetime.Singleton, defaultImplement,
            relationName, isEnabled);

    public static void AddSingleton<TService>(this IDepository depository, object? defaultImplement = null,
            string? relationName = null, bool isEnabled = true) =>
        Add<TService>(depository, DependencyLifetime.Singleton, defaultImplement, relationName, isEnabled);

    public static void AddScoped<TDependency, TImplement>(this IDepository depository,
        string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        Add<TDependency, TImplement>(depository, DependencyLifetime.Scoped, null, relationName, isEnabled);

    public static void AddScoped<TService>(this IDepository depository, string? relationName = null,
        bool isEnabled = true) =>
        Add<TService>(depository, DependencyLifetime.Scoped, null, relationName, isEnabled);

    public static void AddTransient<TDependency, TImplement>(this IDepository depository,
        string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        Add<TDependency, TImplement>(depository, DependencyLifetime.Transient, null, relationName,
            isEnabled);

    public static void AddTransient<TService>(this IDepository depository, string? relationName = null,
        bool isEnabled = true) =>
        Add<TService>(depository, DependencyLifetime.Transient, null, relationName, isEnabled);

    public static void Add<TDependency, TImplement>(this IDepository depository, DependencyLifetime lifetime,
        object? defaultImplement = null, string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        Add(depository, typeof(TDependency), typeof(TImplement), lifetime, defaultImplement, relationName,
            isEnabled);

    public static void Add<TService>(this IDepository depository, DependencyLifetime lifetime,
        object? defaultImplement = null, string? relationName = null, bool isEnabled = true) =>
        Add(depository, typeof(TService), typeof(TService), lifetime, defaultImplement, relationName,
            isEnabled);

    public static void AddSingleton(this IDepository depository, Type dependencyType, Type implementType,
        object? defaultImplement = null, string? relationName = null, bool isEnabled = true)
        => Add(depository, dependencyType, implementType, DependencyLifetime.Singleton, defaultImplement,
            relationName, isEnabled);

    public static void AddTransient(this IDepository depository, Type dependencyType, Type implementType,
        string? relationName = null, bool isEnabled = true)
        => Add(depository, dependencyType, implementType, DependencyLifetime.Transient, null, relationName,
            isEnabled);

    public static void AddScoped(this IDepository depository, Type dependencyType, Type implementType,
        string? relationName = null, bool isEnabled = true)
        => Add(depository, dependencyType, implementType, DependencyLifetime.Scoped, null, relationName,
            isEnabled);

    public static void Add(this IDepository depository, Type dependencyType, Type implementType,
        DependencyLifetime lifetime, object? defaultImplement = null, string? relationName = null,
        bool isEnabled = true)
    {
        var dependencyDescription = depository.GetDependency(dependencyType);

        if (dependencyDescription is null)
        {
            dependencyDescription = new DependencyDescription(DependencyType: dependencyType,
                Lifetime: lifetime);
            depository.AddDependency(dependencyDescription);
        }

        depository.AddRelation(dependencyDescription,
            new DependencyRelation(ImplementType: implementType,
                DefaultImplementation: defaultImplement, Name: relationName, IsEnabled: isEnabled));
    }
}