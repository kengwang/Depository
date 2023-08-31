using Depository.Abstraction.Enums;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Depository.Extensions;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using Xunit;

namespace Depository.Tests;

public class DepositoryResolveTests
{
    // Pure

    [Fact]
    public async void ResolveSingleRegisteredService_ToSingleResolve_ShouldReturnEmptyGuidGenerator()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
                                                    lifetime: DependencyLifetime.Singleton);
        depository.AddDependency(description);
        depository.AddRelation(description,
                               new DependencyRelation(
                                   ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));


        // Action
        var guidGenerator = depository.ResolveDependency(typeof(IGuidGenerator));

        // Assert
        guidGenerator.Should().NotBeNull().And.BeOfType<RandomGuidGenerator>();
    }

    [Fact]
    public async void ResolveAsyncConstructorService_ShouldBeNormal()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton(typeof(IAsyncConstructService), typeof(AsyncConstructorService));

        // Action
        var service = depository.Resolve<IAsyncConstructService>();

        // Assert
        service.As<ICheckIsNormal>().IsNormal.Should().BeTrue();
    }

    [Fact]
    public async void ResolveSingleRegisteredService_InScope_ToSingleResolve_ShouldReturnEmptyGuidGenerator()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
                                                    lifetime: DependencyLifetime.Scoped);
        depository.AddDependency(description);
        depository.AddRelation(description,
                               new DependencyRelation(
                                   ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));


        // Action
        using (var scopeA = DepositoryResolveScope.Create())
        using (var scopeB = DepositoryResolveScope.Create())
        {
            var guidGeneratorA1 = depository.ResolveDependency(typeof(IGuidGenerator), new DependencyResolveOption()
                                                                   {
                                                                       Scope = scopeA
                                                                   });

            var guidGeneratorA2 = depository.ResolveDependency(typeof(IGuidGenerator), new DependencyResolveOption()
                                                                   {
                                                                       Scope = scopeA
                                                                   });

            var guidGeneratorB1 = depository.ResolveDependency(typeof(IGuidGenerator), new DependencyResolveOption()
                                                                   {
                                                                       Scope = scopeB
                                                                   });

            var guidGeneratorB2 = depository.ResolveDependency(typeof(IGuidGenerator), new DependencyResolveOption()
                                                                   {
                                                                       Scope = scopeB
                                                                   });

            // Assert
            guidGeneratorA1.Should().Be(guidGeneratorA2);
            guidGeneratorB1.Should().Be(guidGeneratorB2);
            guidGeneratorA1.Should().NotBe(guidGeneratorB1);
        }
    }

    [Fact]
    public async void ResolveMultipleRegisteredService_ToSingleResolve_ShouldReturnRandomGuidGenerator()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
                                                    lifetime: DependencyLifetime.Singleton);
        depository.AddDependency(description);
        depository.AddRelation(description,
                               new DependencyRelation(ImplementType: typeof(EmptyGuidGenerator),
                                                      DefaultImplementation: null));
        depository.AddRelation(description,
                               new DependencyRelation(
                                   ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));


        // Action
        var guidGenerator = depository.ResolveDependency(typeof(IGuidGenerator));

        // Assert
        guidGenerator.Should().NotBeNull()
                     .And.BeAssignableTo<IGuidGenerator>()
                     .And.BeOfType<RandomGuidGenerator>();
    }


    [Fact]
    public async void ResolveMultipleRegisteredService_ToMultipleResolves_ShouldReturnMultipleGuidGenerators()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
                                                    lifetime: DependencyLifetime.Singleton);
        depository.AddDependency(description);
        depository.AddRelation(description,
                               new DependencyRelation(ImplementType: typeof(EmptyGuidGenerator),
                                                      DefaultImplementation: null));
        depository.AddRelation(description,
                               new DependencyRelation(
                                   ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));


        // Action
        var guidGenerator = depository.ResolveDependencies(typeof(IGuidGenerator));

        // Assert
        guidGenerator.Should().NotBeNull()
                     .And.NotBeEmpty()
                     .And.ContainItemsAssignableTo<IGuidGenerator>()
                     .And.HaveCount(2);
    }

    [Fact]
    public async void ResolveMultipleRegisteredService_ToIEnumerable_ShouldReturnMultipleGuidGenerators()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
                                                    lifetime: DependencyLifetime.Singleton);
        depository.AddDependency(description);
        depository.AddRelation(description,
                               new DependencyRelation(ImplementType: typeof(EmptyGuidGenerator),
                                                      DefaultImplementation: null));
        depository.AddRelation(description,
                               new DependencyRelation(
                                   ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));


        // Action
        var guidGenerator = depository.ResolveDependency(typeof(IEnumerable<IGuidGenerator>));

        // Assert
        var guidGeneratorList = Assert.IsAssignableFrom<IEnumerable<IGuidGenerator>>(guidGenerator).ToList();
        guidGeneratorList.Should().NotBeEmpty()
                         .And.ContainItemsAssignableTo<IGuidGenerator>()
                         .And.HaveCount(2);
    }

    [Fact]
    public async void ResolveService_ToDefaultImplement_ShouldReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
                                                    lifetime: DependencyLifetime.Singleton);
        depository.AddDependency(description);
        var emptyGuidGenerator = new EmptyGuidGenerator();
        depository.AddRelation(description,
                               new DependencyRelation(ImplementType: typeof(EmptyGuidGenerator),
                                                      DefaultImplementation: emptyGuidGenerator));

        // Action
        var resolveGuidGenerator = depository.ResolveDependency(typeof(IGuidGenerator));

        // Assert
        resolveGuidGenerator.Should().Be(emptyGuidGenerator);
    }

    [Fact]
    public async void ResolveMultipleService_ToDefaultImplements_UsingResolves_ShouldAllReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
                                                    lifetime: DependencyLifetime.Singleton);
        depository.AddDependency(description);
        var emptyGuidGenerator = new EmptyGuidGenerator();
        depository.AddRelation(description,
                               new DependencyRelation(ImplementType: typeof(EmptyGuidGenerator),
                                                      DefaultImplementation: emptyGuidGenerator));

        depository.AddRelation(description,
                               new DependencyRelation(
                                   ImplementType: typeof(RandomGuidGenerator),
                                   DefaultImplementation: emptyGuidGenerator));

        // Action
        var resolveGuidGenerator = depository.ResolveDependencies(typeof(IGuidGenerator));

        // Assert
        resolveGuidGenerator.Should().AllSatisfy(item => item.Should().Be(emptyGuidGenerator));
    }

    [Fact]
    public async void ResolveMultipleService_ToDefaultImplement_UsingIEnumerable_ShouldAllReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(dependencyType: typeof(IGuidGenerator),
                                                    lifetime: DependencyLifetime.Singleton);
        depository.AddDependency(description);
        var emptyGuidGenerator = new EmptyGuidGenerator();
        depository.AddRelation(description,
                               new DependencyRelation(ImplementType: typeof(EmptyGuidGenerator),
                                                      DefaultImplementation: emptyGuidGenerator));

        depository.AddRelation(description,
                               new DependencyRelation(
                                   ImplementType: typeof(RandomGuidGenerator),
                                   DefaultImplementation: emptyGuidGenerator));

        // Action
        var resolveGuidGenerator = depository.ResolveDependency(typeof(IEnumerable<IGuidGenerator>));

        // Assert
        Assert.IsAssignableFrom<IEnumerable<IGuidGenerator>>(resolveGuidGenerator).ToList()
              .Should().AllSatisfy(item => item.Should().Be(emptyGuidGenerator));
    }

    // Extensions

    [Fact]
    public async void ResolveSingleRegisteredService_ToSingleResolve_UsingExtension_ShouldReturnEmptyGuidGenerator()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();

        // Action
        var guidGenerator = depository.Resolve<IGuidGenerator>();

        // Assert
        guidGenerator.Should().NotBeNull()
                     .And.BeAssignableTo<IGuidGenerator>();
    }

    [Fact]
    public async void ResolveMultipleRegisteredService_ToSingleResolve_UsingExtension_ShouldReturnRandomGuidGenerator()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();

        // Action
        var guidGenerator = depository.Resolve<IGuidGenerator>();

        // Assert
        guidGenerator.Should().NotBeNull()
                     .And.BeAssignableTo<IGuidGenerator>()
                     .And.BeOfType<RandomGuidGenerator>();
    }

    [Fact]
    public async void
        ResolveMultipleRegisteredService_ToMultipleResolves_UsingExtension_ShouldReturnMultipleGuidGenerators()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();

        // Action
        var guidGenerator = depository.ResolveMultiple<IGuidGenerator>();

        // Assert
        guidGenerator.Should().NotBeNull()
                     .And.NotBeEmpty()
                     .And.ContainItemsAssignableTo<IGuidGenerator>()
                     .And.HaveCount(2);
    }

    [Fact]
    public async void ResolveMultipleRegisteredService_ToIEnumerable_UsingExtension_ShouldReturnMultipleGuidGenerators()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();

        // Action
        var guidGenerator = depository.Resolve<IEnumerable<IGuidGenerator>>();

        // Assert
        guidGenerator.Should().NotBeNull()
                     .And.NotBeEmpty()
                     .And.ContainItemsAssignableTo<IGuidGenerator>()
                     .And.HaveCount(2);
    }

    [Fact]
    public async void ResolveGeneric_ToNormalType_ShouldReturnNormalType()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton<ITypeGeneric<string>, TypeGeneric<string>>();

        // Action
        var guidGenerator = depository.Resolve<ITypeGeneric<string>>();

        // Assert
        guidGenerator.Should().NotBeNull()
                     .And.BeAssignableTo<ITypeGeneric<string>>()
                     .And.BeOfType<TypeGeneric<string>>();
    }

    [Fact]
    public async void ResolveGeneric_ToNormalType_ShouldReturnGenericType()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton(typeof(ITypeGeneric<>), typeof(TypeGeneric<>));

        // Action
        var guidGenerator = depository.Resolve<ITypeGeneric<string>>();

        // Assert
        guidGenerator.Should().NotBeNull()
                     .And.BeAssignableTo<ITypeGeneric<string>>()
                     .And.BeOfType<TypeGeneric<string>>();
    }

    [Fact]
    public async void ResolveConstructorInject_ShouldBeNormal()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton(typeof(IConstructorInjectService), typeof(ConstructorInjectService));
        depository.AddSingleton(typeof(IGuidGenerator), typeof(RandomGuidGenerator));

        // Action
        var service = depository.Resolve<IConstructorInjectService>();

        // Assert
        service.As<ICheckIsNormal>().IsNormal.Should().BeTrue();
    }

    [Fact]
    public async void ResolveIEnumerableConstructorInject_ShouldBeNormal()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton(typeof(IConstructorInjectService),
                                typeof(ConstructorIEnumerableInjectService));
        depository.AddSingleton(typeof(IGuidGenerator), typeof(RandomGuidGenerator));
        depository.AddSingleton(typeof(IGuidGenerator), typeof(EmptyGuidGenerator));

        // Action
        var service = depository.Resolve<IEnumerable<IConstructorInjectService>>();

        // Assert
        service.Should().AllSatisfy(t => t.As<ICheckIsNormal>().IsNormal.Should().BeTrue());
    }

    [Fact]
    public async void ResolveIEnumerableConstructorInject_NoRegister_ShouldNotThrow()
    {
        // Arrange
        var depository = CreateNewDepository();

        // Action
        var service = depository.Resolve<IEnumerable<IConstructorInjectService>>();

        // Assert
        // Nothing
    }

    [Fact]
    public async void ResolveNullableConstructorInject_NoRegister_ShouldNotThrow()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton<IConstructorInjectService, NullableConstructorInjectService>();

        // Action
        var service = depository.Resolve<IConstructorInjectService?>();

        // Assert
        // Nothing
    }

    [Fact]
    public async void ResolveNullableConstructorInject_Register_ShouldNotThrow()
    {
        // Arrange
        var depository = CreateNewDepository();

        // Action
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        var service = depository.Resolve<IGuidGenerator?>();
        // Assert
        service.Should().BeAssignableTo<IGuidGenerator>();
    }

    [Fact]
    public async void ResolveConstructorNotification_ShouldBeNormal()
    {
        // Arrange
        var depository = CreateNewDepository(option => option.AutoNotifyDependencyChange = true);
        depository.AddSingleton(typeof(IConstructorInjectService),
                                typeof(ConstructorInjectNotifiableService));
        depository.AddSingleton(typeof(INotifyDependencyChanged<IGuidGenerator>),
                                typeof(ConstructorInjectNotifiableService));
        depository.AddSingleton(typeof(IGuidGenerator), typeof(RandomGuidGenerator));


        // Action
        var service = depository.Resolve<IConstructorInjectService>();
        depository.ChangeResolveTarget(typeof(IGuidGenerator), new EmptyGuidGenerator());


        // Assert
        service.As<ICheckIsNormal>().IsNormal.Should().BeTrue();
    }

    [Fact]
    public async void ResolveIEnumerableConstructorNotification_ShouldBeNormal()
    {
        // Arrange
        var depository = CreateNewDepository(option => option.AutoNotifyDependencyChange = true);
        depository.AddSingleton(typeof(IConstructorInjectService),
                                typeof(ConstructorInjectNotifiableService));
        depository.AddSingleton(typeof(INotifyDependencyChanged<IGuidGenerator>),
                                typeof(ConstructorInjectNotifiableService));
        depository.AddSingleton(typeof(IGuidGenerator), typeof(RandomGuidGenerator));
        depository.AddSingleton(typeof(IGuidGenerator), typeof(EmptyGuidGenerator));


        // Action
        var service = depository.Resolve<IConstructorInjectService>();
        depository.ChangeResolveTarget(typeof(IGuidGenerator), new EmptyGuidGenerator());


        // Assert
        service.As<ICheckIsNormal>().IsNormal.Should().BeTrue();
    }

    [Fact]
    public async void ResolveService_ToDefaultImplement_UsingExtension_ShouldReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var emptyGuidGenerator = new EmptyGuidGenerator();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator);

        // Action
        var resolveGuidGenerator = depository.Resolve<IGuidGenerator>();

        // Assert
        resolveGuidGenerator.Should().Be(emptyGuidGenerator);
    }

    [Fact]
    public async void ResolveMultipleService_ToDefaultImplement_UsingExtension_ShouldAllReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var emptyGuidGenerator = new EmptyGuidGenerator();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator);
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>(emptyGuidGenerator);

        // Action
        var resolveGuidGenerator = depository.ResolveMultiple<IGuidGenerator>();

        // Assert
        resolveGuidGenerator.Should().AllSatisfy(item => item.Should().Be(emptyGuidGenerator));
    }

    [Fact]
    public async void
        ResolveMultipleService_ToDefaultImplement_UsingExtension_IEnumerable_ShouldAllReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var emptyGuidGenerator = new EmptyGuidGenerator();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator);
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>(emptyGuidGenerator);

        // Action
        var resolveGuidGenerator = depository.Resolve<IEnumerable<IGuidGenerator>>();

        // Assert
        resolveGuidGenerator.Should().AllSatisfy(item => item.Should().Be(emptyGuidGenerator));
    }


    [Fact]
    public async void ResolveDecoration_ToDecorationType_ShouldUseDecorationTypeWithActualType()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.SetDependencyDecoration<IGuidGenerator, GuidDecorationService>();

        // Action
        var resolved = depository.Resolve<IGuidGenerator>();
        resolved.GetGuid();

        // Assert
        resolved.Should().BeAssignableTo<GuidDecorationService>();
    }

    [Fact]
    public async void ResolveDecoration_ToDecorationType_ShouldUseDecorationTypeWithMultipleActualType()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.SetDependencyDecoration<IGuidGenerator, MultipleGuidDecorationService>();

        // Action
        var resolved = depository.Resolve<IGuidGenerator>();
        resolved.GetGuid();

        // Assert
        resolved.Should().BeAssignableTo<MultipleGuidDecorationService>();
    }

    [Fact]
    public async void ResolveDecoration_ToTypes_ShouldOnlyContainsDecorationType()
    {
        // Arrange
        var depository = CreateNewDepository();
        depository.AddSingleton<IGuidGenerator, EmptyGuidGenerator>();
        depository.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        depository.SetDependencyDecoration<IGuidGenerator, MultipleGuidDecorationService>();

        // Action
        var resolved = depository.Resolve<IEnumerable<IGuidGenerator>>();

        // Assert
        resolved.Should().HaveCount(1).And.AllSatisfy(t => t.Should().BeAssignableTo<MultipleGuidDecorationService>());
    }


    // Actions
    private static Core.Depository CreateNewDepository(Action<DepositoryOption>? options = null) =>
        DepositoryFactory.CreateNew(options);
}