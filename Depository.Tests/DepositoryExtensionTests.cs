using Depository.Abstraction.Enums;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Depository.Extensions;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using TUnit.Core;

namespace Depository.Tests;

public class DepositoryExtensionTests
{
    [Test]
    public void Resolve_WithRelationName_ShouldResolveNamedRelation()
    {
        var randomGuidGenerator = new RandomGuidGenerator();
        var emptyGuidGenerator = new EmptyGuidGenerator();
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>(randomGuidGenerator, "random");
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator, "empty");

        var resolvedGenerator = depository.Resolve<IGuidGenerator>(relationName: "empty");

        resolvedGenerator.Should().BeSameAs(emptyGuidGenerator);
    }

    [Test]
    public void Resolve_WithIncludeDisabledTrue_ShouldResolveDisabledRelation()
    {
        var emptyGuidGenerator = new EmptyGuidGenerator();
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator, isEnabled: false);

        var resolvedGenerator = depository.Resolve<IGuidGenerator>(includeDisabled: true);

        resolvedGenerator.Should().BeSameAs(emptyGuidGenerator);
    }

    [Test]
    public void Resolve_WithFixedImplementation_ShouldUseProvidedConstructorParameter()
    {
        var fixedGenerator = new EmptyGuidGenerator();
        var fixedImplementations = new Dictionary<Type, Dictionary<string, object>>
        {
            [typeof(IGuidGenerator)] = new()
            {
                [string.Empty] = fixedGenerator
            }
        };
        var depository = CreateNewDepository();
        depository.AddTransient<IConstructorInjectService, ConstructorInjectService>();

        var service = depository.Resolve<IConstructorInjectService>(fixedImplementations: fixedImplementations);

        service.Should().BeOfType<ConstructorInjectService>();
    }

    [Test]
    public void ResolveInScope_ShouldUseProvidedScopeForScopedService()
    {
        var depository = CreateNewDepository();
        depository.AddScoped<IGuidGenerator, RandomGuidGenerator>();

        using var scope = depository.CreateScope();
        var firstGenerator = depository.ResolveInScope<IGuidGenerator>(scope);
        var secondGenerator = depository.ResolveInScope<IGuidGenerator>(scope);

        secondGenerator.Should().BeSameAs(firstGenerator);
    }

    [Test]
    public void Resolve_WithExplicitOption_ShouldPassOptionToResolver()
    {
        var emptyGuidGenerator = new EmptyGuidGenerator();
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator, isEnabled: false);
        var option = new DependencyResolveOption
        {
            IncludeDisabled = true
        };

        var resolvedGenerator = depository.Resolve<IGuidGenerator>(option);

        resolvedGenerator.Should().BeSameAs(emptyGuidGenerator);
    }

    [Test]
    public void ResolveMultiple_WithRelationName_ShouldReturnNamedRelationOnly()
    {
        var randomGuidGenerator = new RandomGuidGenerator();
        var emptyGuidGenerator = new EmptyGuidGenerator();
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>(randomGuidGenerator, "random");
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator, "empty");

        var resolvedGenerators = depository.ResolveMultiple<IGuidGenerator>(relationName: "empty");

        resolvedGenerators.Should().ContainSingle()
            .Which.Should().BeSameAs(emptyGuidGenerator);
    }

    [Test]
    public void ResolveMultiple_WithExplicitOption_ShouldPassOptionToResolver()
    {
        var emptyGuidGenerator = new EmptyGuidGenerator();
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator, isEnabled: false);
        var option = new DependencyResolveOption
        {
            IncludeDisabled = true
        };

        var resolvedGenerators = depository.ResolveMultiple<IGuidGenerator>(option);

        resolvedGenerators.Should().ContainSingle()
            .Which.Should().BeSameAs(emptyGuidGenerator);
    }

    [Test]
    public void ResolveMultiple_WithIncludeDisabledFalse_ShouldExcludeDisabledRelations()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(isEnabled: false);

        var resolvedGenerators = depository.ResolveMultiple<IGuidGenerator>();

        resolvedGenerators.Should().ContainSingle()
            .Which.Should().BeOfType<RandomGuidGenerator>();
    }

    [Test]
    public void ResolveMultiple_WithIncludeDisabledTrue_ShouldIncludeDisabledRelations()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(isEnabled: false);

        var resolvedGenerators = depository.ResolveMultiple<IGuidGenerator>(includeDisabled: true);

        resolvedGenerators.Should().HaveCount(2);
        resolvedGenerators.Should().Contain(generator => generator is EmptyGuidGenerator);
    }

    [Test]
    public void ResolveMultipleInScope_ShouldUseProvidedScopeForScopedServices()
    {
        var depository = CreateNewDepository();
        depository.AddScoped<IGuidGenerator, RandomGuidGenerator>();

        using var scope = depository.CreateScope();
        var firstGenerators = depository.ResolveMultipleInScope<IGuidGenerator>(scope);
        var secondGenerators = depository.ResolveMultipleInScope<IGuidGenerator>(scope);

        secondGenerators.Should().ContainSingle()
            .Which.Should().BeSameAs(firstGenerators.Single());
    }

    [Test]
    public void AddTransient_WithTypeOverload_ShouldRegisterTransientRelation()
    {
        var depository = CreateNewDepository();

        depository.AddTransient(typeof(IGuidGenerator), typeof(RandomGuidGenerator), relationName: "transient");

        var dependency = depository.GetDependency(typeof(IGuidGenerator));
        dependency.Should().NotBeNull();
        dependency!.Lifetime.Should().Be(DependencyLifetime.Transient);
        depository.Resolve<IGuidGenerator>(relationName: "transient").Should().BeOfType<RandomGuidGenerator>();
    }

    [Test]
    public void AddScoped_WithTypeOverload_ShouldRegisterScopedRelation()
    {
        var depository = CreateNewDepository();

        depository.AddScoped(typeof(IGuidGenerator), typeof(RandomGuidGenerator), relationName: "scoped");

        var dependency = depository.GetDependency(typeof(IGuidGenerator));
        dependency.Should().NotBeNull();
        dependency!.Lifetime.Should().Be(DependencyLifetime.Scoped);
        using var scope = depository.CreateScope();
        depository.ResolveInScope<IGuidGenerator>(scope, new DependencyResolveOption
        {
            RelationName = "scoped"
        }).Should().BeOfType<RandomGuidGenerator>();
    }

    [Test]
    public void RelationExtensions_ByImplementType_ShouldDisableEnableAndRemoveRelation()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();

        depository.DisableRelation<IGuidGenerator, EmptyGuidGenerator>();

        depository.ResolveMultiple<IGuidGenerator>().Should().ContainSingle()
            .Which.Should().BeOfType<RandomGuidGenerator>();

        depository.EnableRelation<IGuidGenerator, EmptyGuidGenerator>();

        depository.ResolveMultiple<IGuidGenerator>().Should().HaveCount(2);

        depository.RemoveRelation<IGuidGenerator, RandomGuidGenerator>();

        depository.ResolveMultiple<IGuidGenerator>().Should().ContainSingle()
            .Which.Should().BeOfType<EmptyGuidGenerator>();
    }

    [Test]
    public void RelationExtensions_ByRelationName_ShouldDisableAndEnableRelation()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>(relationName: "random");
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(relationName: "empty");

        depository.DisableRelation<IGuidGenerator>("empty");

        depository.ResolveMultiple<IGuidGenerator>().Should().ContainSingle()
            .Which.Should().BeOfType<RandomGuidGenerator>();

        depository.EnableRelation<IGuidGenerator>("empty");

        depository.ResolveMultiple<IGuidGenerator>().Should().HaveCount(2);
    }

    [Test]
    public void ChangeFocusingRelationExtension_ShouldResolveRequestedImplementation()
    {
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();

        depository.ChangeFocusingRelation<IGuidGenerator, RandomGuidGenerator>();

        depository.Resolve<IGuidGenerator>().Should().BeOfType<RandomGuidGenerator>();
    }

    private static Core.Depository CreateNewDepository() => DepositoryFactory.CreateNew();
}
