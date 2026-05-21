using System.Diagnostics.CodeAnalysis;
using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;

namespace Depository.Extensions;

public static class AddDependencyExtension
{
    public static void AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TDependency, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplement>(this IDepository depository,
                                                              object? defaultImplement = null,
                                                             string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        Add<TDependency, TImplement>(depository, DependencyLifetime.Singleton, defaultImplement,
                                     relationName, isEnabled);

    public static void AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TService>(this IDepository depository, object? defaultImplement = null,
                                              string? relationName = null, bool isEnabled = true) =>
        Add<TService>(depository, DependencyLifetime.Singleton, defaultImplement, relationName, isEnabled);

    public static void AddScoped<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TDependency, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplement>(this IDepository depository,
                                                           string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        Add<TDependency, TImplement>(depository, DependencyLifetime.Scoped, null, relationName, isEnabled);

    public static void AddScoped<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TService>(this IDepository depository, string? relationName = null,
                                           bool isEnabled = true) =>
        Add<TService>(depository, DependencyLifetime.Scoped, null, relationName, isEnabled);

    public static void AddTransient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TDependency, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplement>(this IDepository depository,
                                                              string? relationName = null, bool isEnabled = true)
        where TImplement : TDependency =>
        Add<TDependency, TImplement>(depository, DependencyLifetime.Transient, null, relationName,
                                     isEnabled);

    public static void AddTransient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TService>(this IDepository depository, string? relationName = null,
                                              bool isEnabled = true) =>
        Add<TService>(depository, DependencyLifetime.Transient, null, relationName, isEnabled);

    public static void Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TDependency, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplement>(this IDepository depository, DependencyLifetime lifetime,
                                                     object? defaultImplement = null, string? relationName = null,
                                                    bool isEnabled = true)
        where TImplement : TDependency =>
        Add(depository, typeof(TDependency), typeof(TImplement), lifetime, defaultImplement, relationName,
            isEnabled);

    public static void Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TService>(this IDepository depository, DependencyLifetime lifetime,
                                     object? defaultImplement = null, string? relationName = null,
                                     bool isEnabled = true) =>
        Add(depository, typeof(TService), typeof(TService), lifetime, defaultImplement, relationName,
            isEnabled);

    public static void AddSingleton(this IDepository depository,
                                    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependencyType,
                                    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
                                    object? defaultImplement = null, string? relationName = null, bool isEnabled = true)
        => Add(depository, dependencyType, implementType, DependencyLifetime.Singleton, defaultImplement,
               relationName, isEnabled);

    public static void AddTransient(this IDepository depository,
                                    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependencyType,
                                    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
                                    string? relationName = null, bool isEnabled = true)
        => Add(depository, dependencyType, implementType, DependencyLifetime.Transient, null, relationName,
               isEnabled);

    public static void AddScoped(this IDepository depository,
                                  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependencyType,
                                  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
                                 string? relationName = null, bool isEnabled = true)
        => Add(depository, dependencyType, implementType, DependencyLifetime.Scoped, null, relationName,
               isEnabled);

    public static void Add(this IDepository depository,
                           [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependencyType,
                           [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementType,
                           DependencyLifetime lifetime, object? defaultImplement = null, string? relationName = null,
                           bool isEnabled = true, Func<IDepository, object>? implementationFactory = null)
    {
        var dependencyDescription = depository.GetDependency(dependencyType);

        if (dependencyDescription is null)
        {
            dependencyDescription = new DependencyDescription(dependencyType: dependencyType,
                                                              lifetime: lifetime);
            depository.AddDependency(dependencyDescription);
        }

        depository.AddRelation(dependencyDescription,
                               new DependencyRelation(ImplementType: implementType,
                                                      DefaultImplementation: defaultImplement, Name: relationName,
                                                      IsEnabled: isEnabled, ImplementationFactory: implementationFactory));
    }

    public static void SetDependencyDecoration<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TDependency, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TDecoration>(
        this IDepository depository, DependencyLifetime lifetime = DependencyLifetime.Singleton)
        where TDecoration : IDecorationService, TDependency
    {
        depository.SetDependencyDecoration(typeof(TDependency), typeof(TDecoration), lifetime);
    }

    public static void SetDependencyDecoration(this IDepository depository,
                                               [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type dependencyType,
                                               [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type decorationType,
                                               DependencyLifetime lifetime = DependencyLifetime.Singleton)
    {
        var dependencyDescription = depository.GetDependency(dependencyType);

        if (dependencyDescription is null)
        {
            dependencyDescription = new DependencyDescription(dependencyType: dependencyType, lifetime: lifetime);
            depository.AddDependency(dependencyDescription);
        }

        var relation = new DependencyRelation(decorationType, IsDecorationRelation: true);
        depository.SetDependencyDecoration(dependencyDescription, relation);
    }
}