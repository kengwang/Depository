using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Models;
using Depository.Core;
using Depository.Extensions;
using FluentAssertions;
using TUnit.Core;

namespace Depository.Tests;

public class DepositoryConstructorResolveTests
{
    [Test]
    public void Resolve_WithActivatorConstructorAttribute_ShouldSelectMarkedConstructor()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<ActivatorConstructorTarget>();

        var service = depository.Resolve<ActivatorConstructorTarget>();

        service.SelectedConstructor.Should().Be("activator");
    }

    [Test]
    public void Resolve_WithMultipleConstructors_ShouldSelectMostResolvableConstructor()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<ConstructorDependency>();
        depository.AddSingleton<MultiConstructorTarget>();

        var service = depository.Resolve<MultiConstructorTarget>();

        service.SelectedConstructor.Should().Be("dependency");
        service.Dependency.Should().BeOfType<ConstructorDependency>();
    }

    [Test]
    public void Resolve_WithUnresolvableDependency_ShouldThrowInitializationException()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<UnresolvableConstructorTarget>();

        var action = () => depository.Resolve<UnresolvableConstructorTarget>();

        action.Should().Throw<DependencyInitializationException>()
            .WithMessage("*cannot resolved*");
    }

    [Test]
    public void Resolve_WithOptionalParameters_ShouldUseDefaultValues()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<OptionalConstructorTarget>();

        var service = depository.Resolve<OptionalConstructorTarget>();

        service.Text.Should().Be("default-text");
        service.Count.Should().Be(42);
    }

    public sealed class ActivatorConstructorTarget
    {
        public ActivatorConstructorTarget()
        {
            SelectedConstructor = "default";
        }

        [DepositoryActivatorConstructor]
        public ActivatorConstructorTarget(string value = "activator")
        {
            SelectedConstructor = value;
        }

        public string SelectedConstructor { get; }
    }

    public sealed class ConstructorDependency;

    public sealed class UnregisteredConstructorDependency;

    public sealed class MultiConstructorTarget
    {
        public MultiConstructorTarget()
        {
            SelectedConstructor = "default";
        }

        public MultiConstructorTarget(ConstructorDependency dependency)
        {
            SelectedConstructor = "dependency";
            Dependency = dependency;
        }

        public MultiConstructorTarget(ConstructorDependency dependency, UnregisteredConstructorDependency unregisteredDependency)
        {
            SelectedConstructor = "unregistered";
            Dependency = dependency;
            UnregisteredDependency = unregisteredDependency;
        }

        public string SelectedConstructor { get; }
        public ConstructorDependency? Dependency { get; }
        public UnregisteredConstructorDependency? UnregisteredDependency { get; }
    }

    public sealed class UnresolvableConstructorTarget
    {
        public UnresolvableConstructorTarget(UnregisteredConstructorDependency dependency)
        {
            Dependency = dependency;
        }

        public UnregisteredConstructorDependency Dependency { get; }
    }

    public sealed class OptionalConstructorTarget
    {
        public OptionalConstructorTarget(string text = "default-text", int count = 42)
        {
            Text = text;
            Count = count;
        }

        public string Text { get; }
        public int Count { get; }
    }
}
