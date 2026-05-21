using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;
using Depository.Core;
using Depository.Extensions;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using TUnit.Core;

namespace Depository.Tests;

public class DepositoryAddTests
{
    // Pure
    [Test]
    public void AddServiceAsSingleton_ShouldBeResolved()
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

    [Test]
    public void AddServiceAsTransient_ShouldBeResolved()
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

    [Test]
    public void AddServiceAsScoped_ShouldBeResolved()
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

    [Test]
    public void AddMultipleRelationsSingleton_ShouldBeResolvedToLastServices()
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

    [Test]
    public void AddServiceExtensionSingletonToSelf_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(RandomGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(RandomGuidGenerator),
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Test]
    public void AddServiceExtensionTransientToSelf_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddTransient<RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(RandomGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(RandomGuidGenerator),
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Test]
    public void AddServiceExtensionScopedToSelf_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddScoped<RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(RandomGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(RandomGuidGenerator),
            DependencyLifetime.Scoped);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Test]
    public void AddServiceExtensionSingletonToImpl_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator),
            DependencyLifetime.Singleton);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Test]
    public void AddServiceExtensionTransientToImpl_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddTransient<IGuidGenerator, RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator),
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Test]
    public void AddServiceExtensionScopedToImpl_ShouldBeResolved()
    {
        var depository = CreateNewDepository();
        depository.AddTransient<IGuidGenerator, RandomGuidGenerator>();
        var resolvedDependency = depository.GetDependency(typeof(IGuidGenerator));
        var resolvedRelation = depository.GetRelation(resolvedDependency!);
        AssertDepDescIfMatch(resolvedDependency, typeof(IGuidGenerator),
            DependencyLifetime.Transient);
        AssertDepRelationIfMatch(resolvedRelation, typeof(RandomGuidGenerator), null!);
    }

    [Test]
    public void DeleteRelation_ShouldRemoveOnlyTargetRelation()
    {
        var depository = CreateNewDepository();
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        var randomRelation = new DependencyRelation(typeof(RandomGuidGenerator));
        var emptyRelation = new DependencyRelation(typeof(EmptyGuidGenerator));
        depository.AddDependency(description);
        depository.AddRelation(description, randomRelation);
        depository.AddRelation(description, emptyRelation);

        depository.DeleteRelation(description, randomRelation);

        depository.GetRelations(description).Should().ContainSingle()
            .Which.Should().Be(emptyRelation);
    }

    [Test]
    public void ClearRelations_ShouldRemoveAllRelations()
    {
        var depository = CreateNewDepository();
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        depository.AddDependency(description);
        depository.AddRelation(description, new DependencyRelation(typeof(RandomGuidGenerator)));

        depository.ClearRelations(description);

        depository.GetRelations(description).Should().BeEmpty();
        var action = () => depository.GetRelation(description);
        action.Should().Throw<RelationNotFoundException>();
    }

    [Test]
    public void DisableAndEnableRelation_ShouldRespectIncludeDisabledOption()
    {
        var depository = CreateNewDepository();
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        var relation = new DependencyRelation(typeof(RandomGuidGenerator));
        depository.AddDependency(description);
        depository.AddRelation(description, relation);

        depository.DisableRelation(description, relation);

        depository.GetRelations(description).Should().BeEmpty();
        depository.GetRelations(description, includeDisabled: true).Should().ContainSingle()
            .Which.Should().Be(relation);

        depository.EnableRelation(description, relation);

        depository.GetRelations(description).Should().ContainSingle()
            .Which.Should().Be(relation);
    }

    [Test]
    public void ChangeFocusingRelation_ShouldResolveFocusedRelation()
    {
        var depository = CreateNewDepository();
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        var randomRelation = new DependencyRelation(typeof(RandomGuidGenerator));
        var emptyRelation = new DependencyRelation(typeof(EmptyGuidGenerator));
        depository.AddDependency(description);
        depository.AddRelation(description, randomRelation);
        depository.AddRelation(description, emptyRelation);

        depository.ChangeFocusingRelation(description, randomRelation);

        depository.GetRelation(description).Should().Be(randomRelation);
    }

    [Test]
    public void GetRelation_WithUnknownName_ShouldThrowDependencyNotFoundException()
    {
        var depository = CreateNewDepository();
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        depository.AddDependency(description);
        depository.AddRelation(description, new DependencyRelation(typeof(RandomGuidGenerator), Name: "known"));

        var action = () => depository.GetRelation(description, relationName: "missing");

        action.Should().Throw<DependencyNotFoundException>();
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
