using Depository.Abstraction.Enums;
using Depository.Abstraction.Exceptions;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Depository.Extensions;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using TUnit.Core;

namespace Depository.Tests;

public class DepositoryResolveEdgeTests
{
    [Test]
    public void ResolveDependencies_ForOpenGeneric_ShouldResolveClosedImplementations()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton(typeof(ITypeGeneric<>), typeof(TypeGeneric<>));
        depository.AddSingleton(typeof(ITypeGeneric<>), typeof(StringTypeGeneric));

        var services = depository.ResolveDependencies(typeof(ITypeGeneric<string>));

        services.Should().HaveCount(2);
        services.Should().AllSatisfy(service => service.Should().BeAssignableTo<ITypeGeneric<string>>());
        services.Should().Contain(service => service is TypeGeneric<string>);
        services.Should().Contain(service => service is StringTypeGeneric);
    }

    [Test]
    public void ResolveDependencies_ForMissingOpenGenericWithNoThrow_ShouldReturnEmptyList()
    {
        var depository = CreateNewDepository();

        var services = depository.ResolveDependencies(typeof(ITypeGeneric<int>), new DependencyResolveOption
        {
            ThrowWhenNotExists = false
        });

        services.Should().BeEmpty();
    }

    [Test]
    public void ResolveDependency_ForMissingNullableWithNoThrow_ShouldReturnNull()
    {
        var depository = CreateNewDepository();

        var service = depository.ResolveDependency(typeof(Nullable<int>), new DependencyResolveOption
        {
            ThrowWhenNotExists = false
        });

        service.Should().BeNull();
    }

    [Test]
    public void ResolveDependency_ForMissingTaskWithNoThrow_ShouldReturnNull()
    {
        var depository = CreateNewDepository();

        var service = depository.ResolveDependency(typeof(Task<IGuidGenerator>), new DependencyResolveOption
        {
            ThrowWhenNotExists = false
        });

        service.Should().BeNull();
    }

    [Test]
    public void ResolveDependency_ForMissingTaskWithThrow_ShouldThrowDependencyNotFoundException()
    {
        var depository = CreateNewDepository();

        var action = () => depository.ResolveDependency(typeof(Task<IGuidGenerator>), new DependencyResolveOption
        {
            ThrowWhenNotExists = true
        });

        action.Should().Throw<DependencyNotFoundException>();
    }

    [Test]
    public async Task ResolveDependency_ForTaskOfNormalService_ShouldReturnCompletedTask()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();

        var task = depository.Resolve<Task<IGuidGenerator>>();
        var service = await task;

        service.Should().BeOfType<EmptyGuidGenerator>();
    }

    [Test]
    public void ResolveDependency_WithSkipDecoration_ShouldResolveOriginalRelation()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.SetDependencyDecoration<IGuidGenerator, GuidDecorationService>();

        var service = depository.Resolve<IGuidGenerator>(new DependencyResolveOption
        {
            SkipDecoration = true
        });

        service.Should().BeOfType<EmptyGuidGenerator>();
    }

    [Test]
    public void ResolveDependencies_WithSkipDecoration_ShouldResolveOriginalRelations()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.SetDependencyDecoration<IGuidGenerator, GuidDecorationService>();

        var services = depository.ResolveDependencies(typeof(IGuidGenerator), new DependencyResolveOption
        {
            SkipDecoration = true
        });

        services.Should().ContainSingle()
            .Which.Should().BeOfType<EmptyGuidGenerator>();
    }

    [Test]
    public void ResolveParameterInfos_WithDefaultValue_ShouldUseDefaultParameterValue()
    {
        var depository = CreateNewDepository();
        var constructor = typeof(DefaultValueConstructorService).GetConstructors().Single();

        var parameters = depository.ResolveParameterInfos(
            typeof(DefaultValueConstructorService),
            constructor.GetParameters(),
            null);

        parameters.Should().ContainSingle()
            .Which.Should().Be("default-value");
    }

    [Test]
    public void ResolveParameterInfos_WithOptionalReference_ShouldUseNullValue()
    {
        var depository = CreateNewDepository();
        var constructor = typeof(OptionalReferenceConstructorService).GetConstructors().Single();

        var parameters = depository.ResolveParameterInfos(
            typeof(OptionalReferenceConstructorService),
            constructor.GetParameters(),
            null);

        parameters.Should().ContainSingle()
            .Which.Should().BeNull();
    }

    [Test]
    public void ResolveParameterInfos_WithUnresolvableRequiredParameter_ShouldThrowInitializationException()
    {
        var depository = CreateNewDepository();
        var constructor = typeof(ConstructorInjectService).GetConstructors().Single();

        var action = () => depository.ResolveParameterInfos(
            typeof(ConstructorInjectService),
            constructor.GetParameters(),
            null);

        action.Should().Throw<DependencyInitializationException>();
    }

    [Test]
    public void ResolveDependencies_ForGenericDefaultImplementation_ShouldReturnDefaultImplementation()
    {
        var depository = CreateNewDepository();
        var defaultImplementation = new TypeGeneric<string>();
        var description = new DependencyDescription(typeof(ITypeGeneric<>), DependencyLifetime.Singleton);
        depository.AddDependency(description);
        depository.AddRelation(description, new DependencyRelation(typeof(TypeGeneric<>), defaultImplementation));

        var services = depository.ResolveDependencies(typeof(ITypeGeneric<string>));

        services.Should().ContainSingle()
            .Which.Should().BeSameAs(defaultImplementation);
    }

    [Test]
    public void ResolveDependencies_ForGenericImplementationFactory_ShouldReturnFactoryResult()
    {
        var depository = CreateNewDepository();
        var factoryImplementation = new TypeGeneric<string>();
        var description = new DependencyDescription(typeof(ITypeGeneric<>), DependencyLifetime.Singleton);
        depository.AddDependency(description);
        depository.AddRelation(description, new DependencyRelation(
            typeof(TypeGeneric<>),
            ImplementationFactory: _ => factoryImplementation));

        var services = depository.ResolveDependencies(typeof(ITypeGeneric<string>));

        services.Should().ContainSingle()
            .Which.Should().BeSameAs(factoryImplementation);
    }


    [Test]
    public void ResolveInScope_WithExistingOption_ShouldNotMutateOptionScope()
    {
        var depository = CreateNewDepository();
        depository.AddScoped<IGuidGenerator, EmptyGuidGenerator>();
        using var scope = DepositoryResolveScope.Create();
        var option = new DependencyResolveOption();

        depository.ResolveInScope<IGuidGenerator>(scope, option);

        option.Scope.Should().BeNull();
    }

    [Test]
    public void ResolveScoped_WithNamedRelationsUsingSameImplementation_ShouldKeepSeparateScopedInstances()
    {
        var depository = CreateNewDepository();
        depository.AddScoped<IGuidGenerator, RandomGuidGenerator>("a");
        depository.AddScoped<IGuidGenerator, RandomGuidGenerator>("b");
        using var scope = DepositoryResolveScope.Create();

        var firstA = depository.ResolveInScope<IGuidGenerator>(scope,
            new DependencyResolveOption { RelationName = "a" });
        var secondA = depository.ResolveInScope<IGuidGenerator>(scope,
            new DependencyResolveOption { RelationName = "a" });
        var firstB = depository.ResolveInScope<IGuidGenerator>(scope,
            new DependencyResolveOption { RelationName = "b" });

        secondA.Should().BeSameAs(firstA);
        firstB.Should().NotBeSameAs(firstA);
    }

    [Test]
    public void ResolveDependency_WithRelationName_ShouldIgnoreFocusedRelation()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(relationName: "empty");
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>(relationName: "random");
        depository.ChangeFocusingRelation<IGuidGenerator, RandomGuidGenerator>();

        var service = depository.Resolve<IGuidGenerator>(new DependencyResolveOption { RelationName = "empty" });

        service.Should().BeOfType<EmptyGuidGenerator>();
    }

    [Test]
    public void ResolveDependency_ForOpenGenericDecoration_ShouldCloseDecoratorImplementation()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton(typeof(ITypeGeneric<>), typeof(TypeGeneric<>));
        var description = depository.GetDependency(typeof(ITypeGeneric<>))!;
        depository.SetDependencyDecoration(description, new DependencyRelation(
            typeof(GenericDecorationService<>),
            IsDecorationRelation: true));

        var service = depository.Resolve<ITypeGeneric<string>>();

        service.Should().BeOfType<GenericDecorationService<string>>();
        service.GetGenericType().Should().Be(typeof(string));
    }



    [Test]
    public void ResolveTaskDependency_WithExistingOption_ShouldNotMutateCheckAsyncConstructor()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        var option = new DependencyResolveOption
        {
            CheckAsyncConstructor = true
        };

        depository.Resolve<Task<IGuidGenerator>>(option);

        option.CheckAsyncConstructor.Should().BeTrue();
    }

    [Test]
    public void ResolveDependency_WithDisabledRelationName_ShouldRespectIncludeDisabled()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(relationName: "disabled", isEnabled: false);

        var resolveEnabledOnly = () => depository.Resolve<IGuidGenerator>(new DependencyResolveOption
        {
            RelationName = "disabled"
        });
        var resolvedDisabled = depository.Resolve<IGuidGenerator>(new DependencyResolveOption
        {
            RelationName = "disabled",
            IncludeDisabled = true
        });

        resolveEnabledOnly.Should().Throw<DependencyNotFoundException>();
        resolvedDisabled.Should().BeOfType<EmptyGuidGenerator>();
    }

    private static Core.Depository CreateNewDepository() => DepositoryFactory.CreateNew();

    private sealed class GenericDecorationService<T> : ITypeGeneric<T>, IDecorationService
    {
        private readonly ITypeGeneric<T> _inner;

        public GenericDecorationService(ITypeGeneric<T> inner)
        {
            _inner = inner;
        }

        public Type GetGenericType()
        {
            return _inner.GetGenericType();
        }
    }

    private sealed class DefaultValueConstructorService
    {
        public DefaultValueConstructorService(string value = "default-value")
        {
            Value = value;
        }

        public string Value { get; }
    }

    private sealed class OptionalReferenceConstructorService
    {
        public OptionalReferenceConstructorService(object? value = null)
        {
            Value = value;
        }

        public object? Value { get; }
    }
}
