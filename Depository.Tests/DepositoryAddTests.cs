using Depository.Abstraction.Enums;
using Depository.Abstraction.Models;
using Depository.Core;
using Depository.Extensions;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using Xunit;

namespace Depository.Tests;

public class DepositoryAddTests
{
    // Pure
    [Fact]
    public async void AddServiceAsSingleton_ShouldBeResolved()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
            lifetime: DependencyLifetime.Singleton);
        depository.AddDependency(description);
        var relation = new DependencyRelation(
            ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null);
        depository.AddRelation(description, relation);

        // Action
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);

        // Assert
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator), DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceAsTransient_ShouldBeResolved()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
            lifetime: DependencyLifetime.Transient);
        depository.AddDependency(description);
        var relation = new DependencyRelation(
            ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null);
        depository.AddRelation(description, relation);

        // Action
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);

        // Assert
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator),
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceAsScoped_ShouldBeResolved()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
            lifetime: DependencyLifetime.Scoped);
        depository.AddDependency(description);
        var relation = new DependencyRelation(
            ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null);
        depository.AddRelation(description, relation);

        // Action
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);

        // Assert
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator),
            DependencyLifetime.Scoped);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddMultipleRelationsSingleton_ShouldBeResolvedToLastServices()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
            lifetime: DependencyLifetime.Singleton);
        depository.AddDependency(description);
        var relation1 = new DependencyRelation(
            ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null);
        var relation2 = new DependencyRelation(
            ImplementType: typeof(EmptyGuidGenerator), DefaultImplementation: null);
        depository.AddRelation(description, relation1);
        depository.AddRelation(description, relation2);

        // Action
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);

        // Assert
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator),
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, typeof(EmptyGuidGenerator), null!);
    }

    // Extensions

    [Fact]
    public async void AddServiceExtensionSingletonToSelf_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(RandomGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(RandomGuidGenerator),
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionTransientToSelf_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddTransient<RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(RandomGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(RandomGuidGenerator),
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionScopedToSelf_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddScoped<RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(RandomGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(RandomGuidGenerator),
            DependencyLifetime.Scoped);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionSingletonToImpl_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator),
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionTransientToImpl_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddTransient<IGuidGenerator, RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator),
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionScopedToImpl_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddTransient<IGuidGenerator, RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator),
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }


    // Actions
    private static Core.Depository CreateNewDepository() => DepositoryFactory.CreateNew();

    private static void AssertDepDescIfMatch(DependencyDescription? resolvedDependency, Type dependencyType,
        DependencyLifetime lifetime)
    {
        resolvedDependency.Should().NotBeNull();
        resolvedDependency!.DependencyType.Should().Be(dependencyType);
        resolvedDependency.Lifetime.Should().Be(lifetime);
    }

    private static void AssertDepRelationIfMatch(DependencyRelation? relation, Type implType, object defaultImpl)
    {
        relation.Should().NotBeNull();
        relation!.ImplementType.Should().Be(implType);
        relation.DefaultImplementation.Should().Be(defaultImpl);
    }
}