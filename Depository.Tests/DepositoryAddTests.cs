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
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        var relation = new DependencyRelation(RelationType: DependencyRelationType.Once,
            ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null);
        await depository.AddRelationAsync(description, relation);

        // Action
        var resolvedDependency = await depository.GetDependencyAsync(typeof(IGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);

        // Assert
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceAsTransient_ShouldBeResolved()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Transient);
        await depository.AddDependencyAsync(description);
        var relation = new DependencyRelation(RelationType: DependencyRelationType.Once,
            ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null);
        await depository.AddRelationAsync(description, relation);

        // Action
        var resolvedDependency = await depository.GetDependencyAsync(typeof(IGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);

        // Assert
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceAsScoped_ShouldBeResolved()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Scoped);
        await depository.AddDependencyAsync(description);
        var relation = new DependencyRelation(RelationType: DependencyRelationType.Once,
            ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null);
        await depository.AddRelationAsync(description, relation);

        // Action
        var resolvedDependency = await depository.GetDependencyAsync(typeof(IGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);

        // Assert
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Scoped);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddMultipleRelationsLastWinSingleton_ShouldBeResolvedToMultipleServices()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        var relation1 = new DependencyRelation(RelationType: DependencyRelationType.Once,
            ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null);
        var relation2 = new DependencyRelation(RelationType: DependencyRelationType.Once,
            ImplementType: typeof(EmptyGuidGenerator), DefaultImplementation: null);
        await depository.AddRelationAsync(description, relation1);
        await depository.AddRelationAsync(description, relation2);

        // Action
        var resolvedDependency = await depository.GetDependencyAsync(typeof(IGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);

        // Assert
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(EmptyGuidGenerator), null!);
    }

    [Fact]
    public async void AddMultipleRelationsFirstWinSingleton_ShouldBeResolvedToMultipleServices()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.FirstWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        var relation1 = new DependencyRelation(RelationType: DependencyRelationType.Once,
            ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null);
        var relation2 = new DependencyRelation(RelationType: DependencyRelationType.Once,
            ImplementType: typeof(EmptyGuidGenerator), DefaultImplementation: null);
        await depository.AddRelationAsync(description, relation1);
        await depository.AddRelationAsync(description, relation2);

        // Action
        var resolvedDependency = await depository.GetDependencyAsync(typeof(IGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);

        // Assert
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator), DependencyResolvePolicy.FirstWin,
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }

    // Extensions

    [Fact]
    public async void AddServiceExtensionSingletonToSelf_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync<RandomGuidGenerator>();
        var resolvedDependency = await depository.GetDependencyAsync(typeof(RandomGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(RandomGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionTransientToSelf_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        await depository.AddTransientAsync<RandomGuidGenerator>();
        var resolvedDependency = await depository.GetDependencyAsync(typeof(RandomGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(RandomGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionScopedToSelf_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        await depository.AddScopedAsync<RandomGuidGenerator>();
        var resolvedDependency = await depository.GetDependencyAsync(typeof(RandomGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(RandomGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Scoped);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionSingletonToImpl_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();
        var resolvedDependency = await depository.GetDependencyAsync(typeof(IGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionTransientToImpl_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        await depository.AddTransientAsync<IGuidGenerator, RandomGuidGenerator>();
        var resolvedDependency = await depository.GetDependencyAsync(typeof(IGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }

    [Fact]
    public async void AddServiceExtensionScopedToImpl_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        await depository.AddTransientAsync<IGuidGenerator, RandomGuidGenerator>();
        var resolvedDependency = await depository.GetDependencyAsync(typeof(IGuidGenerator));
        var resolvedRelation = await depository.GetRelationAsync(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator), DependencyResolvePolicy.LastWin,
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, DependencyRelationType.Once, typeof(RandomGuidGenerator), null!);
    }


    // Actions
    private static Core.Depository CreateNewDepository() => DepositoryFactory.CreateNew();

    private static void AssertDepDescIfMatch(DependencyDescription? resolvedDependency, Type dependencyType,
        DependencyResolvePolicy policy, DependencyLifetime lifetime)
    {
        resolvedDependency.Should().NotBeNull();
        resolvedDependency!.DependencyType.Should().Be(dependencyType);
        resolvedDependency.ResolvePolicy.Should().Be(policy);
        resolvedDependency.Lifetime.Should().Be(lifetime);
    }

    private static void AssertDepRelationIfMatch(DependencyRelation? relation, DependencyRelationType relationType,
        Type implType, object defaultImpl)
    {
        relation.Should().NotBeNull();
        relation!.RelationType.Should().Be(relationType);
        relation.ImplementType.Should().Be(implType);
        relation.DefaultImplementation.Should().Be(defaultImpl);
    }
}