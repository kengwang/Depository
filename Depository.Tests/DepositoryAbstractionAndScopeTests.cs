using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using TUnit.Core;

namespace Depository.Tests;

public class DepositoryAbstractionAndScopeTests
{
    [Test]
    public void DependencyDescription_EqualsSameInstance_ShouldReturnTrue()
    {
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);

        description.Equals(description).Should().BeTrue();
    }

    [Test]
    public void DependencyDescription_EqualsSameTypeAndLifetime_ShouldReturnTrue()
    {
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        var sameDescription = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);

        description.Equals(sameDescription).Should().BeTrue();
        description.GetHashCode().Should().Be(sameDescription.GetHashCode());
    }

    [Test]
    public void DependencyDescription_EqualsDifferentLifetime_ShouldReturnFalse()
    {
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        var differentDescription = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Transient);

        description.Equals(differentDescription).Should().BeFalse();
        description.Equals(new object()).Should().BeFalse();
    }

    [Test]
    public void DependencyDescription_Deconstruct_ShouldReturnStoredValues()
    {
        var relation = new DependencyRelation(typeof(RandomGuidGenerator));
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Scoped)
        {
            DecorationRelation = relation
        };

        var (dependencyType, lifetime, decorationRelation) = description;

        dependencyType.Should().Be(typeof(IGuidGenerator));
        lifetime.Should().Be(DependencyLifetime.Scoped);
        decorationRelation.Should().Be(relation);
    }

    [Test]
    public void ResolveScope_DisposeWithAutoDisposeEnabled_ShouldDisposeStoredImplementations()
    {
        var disposable = new DisposableService();
        var scope = DepositoryResolveScope.Create(new DepositoryResolveScopeOption
        {
            AutoDisposeWhenRemoved = true
        });
        scope.SetImplementation(typeof(DisposableService), disposable);

        scope.Dispose();

        disposable.IsDisposed.Should().BeTrue();
        scope.Exist(typeof(DisposableService)).Should().BeFalse();
    }

    [Test]
    public void ResolveScope_RemoveMissingImplementation_ShouldNotThrow()
    {
        var scope = DepositoryResolveScope.Create();

        var action = () => scope.RemoveImplement(typeof(DisposableService));

        action.Should().NotThrow();
    }

    [Test]
    public void Depository_SetAndRemoveImplementation_ShouldUpdateRootScope()
    {
        var depository = DepositoryFactory.CreateNew();
        var generator = new EmptyGuidGenerator();

        depository.SetImplementation(typeof(IGuidGenerator), generator);

        depository.RootScope.GetImplement(typeof(IGuidGenerator)).Should().BeSameAs(generator);

        depository.RemoveImplementation(typeof(IGuidGenerator));

        depository.RootScope.Exist(typeof(IGuidGenerator)).Should().BeFalse();
    }

    [Test]
    public void Depository_DeleteDependency_ShouldRemoveDependencyAndRelations()
    {
        var depository = DepositoryFactory.CreateNew();
        var description = new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton);
        depository.AddDependency(description);
        depository.AddRelation(description, new DependencyRelation(typeof(RandomGuidGenerator)));

        depository.DeleteDependency(description);

        depository.GetDependency(typeof(IGuidGenerator)).Should().BeNull();
        depository.GetRelations(description).Should().BeEmpty();
    }

    [Test]
    public void Depository_ClearAllDependencies_ShouldRemoveRegisteredDependencies()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddDependency(new DependencyDescription(typeof(IGuidGenerator), DependencyLifetime.Singleton));

        depository.ClearAllDependencies();

        depository.GetDependency(typeof(IGuidGenerator)).Should().BeNull();
        depository.GetDependency(typeof(IDepository)).Should().BeNull();
    }

    private sealed class DisposableService : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
