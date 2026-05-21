using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;
using Depository.Extensions;
using Depository.Extensions.DependencyInjection;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core;

namespace Depository.Tests;

public class DepositoryRelationAndProviderEdgeTests
{
    [Test]
    public void AddRelation_WithInheritanceCheckerEnabledAndAssignableImplementation_ShouldThrow()
    {
        var action = () => CreateNewDepository(option =>
            option.CheckerOption.ImplementIsInheritedFromDependency = true);

        action.Should().Throw<ImplementNotInheritedFromDependencyException>();
    }

    [Test]
    public void AddRelation_WithInstantiableCheckerEnabledAndInterfaceImplementation_ShouldThrow()
    {
        var depository = CreateNewDepository(option =>
            option.CheckerOption.ImplementIsInstantiable = true);
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        depository.AddDependency(description);

        var action = () => depository.AddRelation(description, new DependencyRelation(typeof(IGuidGenerator)));

        action.Should().Throw<ImplementNotInstantiableException>();
    }

    [Test]
    public void AddRelation_WithDuplicatedThrowAction_ShouldThrow()
    {
        var depository = CreateNewDepository(option =>
            option.ImplementTypeDuplicatedAction = ImplementTypeDuplicatedAction.Throw);
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        var relation = new DependencyRelation(typeof(RandomGuidGenerator));
        depository.AddDependency(description);
        depository.AddRelation(description, relation);

        var action = () => depository.AddRelation(description, relation);

        action.Should().Throw<ImplementDuplicatedException>();
    }

    [Test]
    public void AddRelation_WithDuplicatedContinueAction_ShouldKeepDuplicates()
    {
        var depository = CreateNewDepository(option =>
            option.ImplementTypeDuplicatedAction = ImplementTypeDuplicatedAction.Continue);
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        var relation = new DependencyRelation(typeof(RandomGuidGenerator), Name: "first");
        var duplicatedImplementRelation = new DependencyRelation(typeof(RandomGuidGenerator), Name: "second");
        depository.AddDependency(description);

        depository.AddRelation(description, relation);
        depository.AddRelation(description, duplicatedImplementRelation);

        depository.GetRelations(description).Should().HaveCount(2);
    }

    [Test]
    public void DeleteRelation_ForUnknownDependency_ShouldNotThrow()
    {
        var depository = CreateNewDepository();
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);

        var action = () => depository.DeleteRelation(description, new DependencyRelation(typeof(RandomGuidGenerator)));

        action.Should().NotThrow();
    }

    [Test]
    public void GetRelation_WithDisabledFocusedRelation_ShouldResolveEnabledFallback()
    {
        var depository = CreateNewDepository();
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        var focusedRelation = new DependencyRelation(typeof(RandomGuidGenerator));
        var fallbackRelation = new DependencyRelation(typeof(EmptyGuidGenerator));
        depository.AddDependency(description);
        depository.AddRelation(description, focusedRelation);
        depository.AddRelation(description, fallbackRelation);
        depository.ChangeFocusingRelation(description, focusedRelation);

        depository.DisableRelation(description, focusedRelation);

        depository.GetRelation(description).Should().Be(fallbackRelation);
    }

    [Test]
    public void ServiceProvider_GetRequiredKeyedService_ShouldResolveNamedService()
    {
        var depository = CreateNewDepository();
        var emptyGuidGenerator = new EmptyGuidGenerator();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator, "empty");
        var provider = new DepositoryServiceProvider(depository);

        var service = provider.GetRequiredKeyedService(typeof(IGuidGenerator), "empty");

        service.Should().BeSameAs(emptyGuidGenerator);
    }

    [Test]
    public void ServiceProvider_GetKeyedService_WhenMissing_ShouldReturnNull()
    {
        var depository = CreateNewDepository();
        var provider = new DepositoryServiceProvider(depository);

        var service = provider.GetKeyedService(typeof(IGuidGenerator), "missing");

        service.Should().BeNull();
    }

    [Test]
    public void ServiceProvider_IsKeyedService_WhenDependencyMissing_ShouldReturnFalse()
    {
        var depository = CreateNewDepository();
        var provider = new DepositoryServiceProvider(depository);

        provider.IsKeyedService(typeof(IGuidGenerator), "missing").Should().BeFalse();
    }

    [Test]
    public void ChangeResolveTarget_ForTransientDependency_ShouldNotifyWithoutChangingRootSingletonCache()
    {
        var depository = CreateNewDepository();
        depository.AddTransient<IGuidGenerator, RandomGuidGenerator>();

        depository.ChangeResolveTarget(typeof(IGuidGenerator), new EmptyGuidGenerator());

        depository.RootScope.Exist(typeof(IGuidGenerator)).Should().BeFalse();
    }

    [Test]
    public void ServiceProvider_CreateScope_ShouldReturnCachedScopedServiceProvider()
    {
        var depository = CreateNewDepository();
        var provider = new DepositoryServiceProvider(depository);

        using var scope = provider.CreateScope();
        var firstProvider = scope.ServiceProvider;
        var secondProvider = scope.ServiceProvider;

        firstProvider.Should().BeSameAs(secondProvider);
    }

    [Test]
    public void SafeToString_ShouldHandleNullStringAndObjectValues()
    {
        Core.Depository.SafeToString(null).Should().Be("null");
        Core.Depository.SafeToString("name").Should().Be("name");

        var serviceKey = new object();

        Core.Depository.SafeToString(serviceKey).Should().Contain(serviceKey.GetHashCode().ToString("X"));
    }

    private static Core.Depository CreateNewDepository(Action<DepositoryOption>? options = null) =>
        Core.DepositoryFactory.CreateNew(options);
}
