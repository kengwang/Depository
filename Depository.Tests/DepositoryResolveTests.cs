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
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once,
                ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));


        // Action
        var guidGenerator = await depository.ResolveDependencyAsync(typeof(IGuidGenerator));

        // Assert
        guidGenerator.Should().NotBeNull().And.BeOfType<RandomGuidGenerator>();
    }
    
    [Fact]
    public async void ResolveAsyncConstructorService_ShouldBeNormal()
    {
        // Arrange
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync(typeof(IAsyncConstructService), typeof(AsyncConstructorService));

        // Action
        var service = await depository.ResolveAsync<IAsyncConstructService>();

        // Assert
        service.As<ICheckIsNormal>().IsNormal.Should().BeTrue();
    }
    
    [Fact]
    public async void ResolveSingleRegisteredService_InScope_ToSingleResolve_ShouldReturnEmptyGuidGenerator()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Scoped);
        await depository.AddDependencyAsync(description);
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once,
                ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));
        

        // Action
        using (var scopeA = DepositoryResolveScope.Create())
        using (var scopeB = DepositoryResolveScope.Create())
        {
            var guidGeneratorA1 = await depository.ResolveDependencyAsync(typeof(IGuidGenerator), new DependencyResolveOption()
            {
                Scope = scopeA
            });
        
            var guidGeneratorA2 = await depository.ResolveDependencyAsync(typeof(IGuidGenerator), new DependencyResolveOption()
            {
                Scope = scopeA
            });
        
            var guidGeneratorB1 = await depository.ResolveDependencyAsync(typeof(IGuidGenerator), new DependencyResolveOption()
            {
                Scope = scopeB
            });
        
            var guidGeneratorB2 = await depository.ResolveDependencyAsync(typeof(IGuidGenerator), new DependencyResolveOption()
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
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once, ImplementType: typeof(EmptyGuidGenerator),
                DefaultImplementation: null));
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once,
                ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));


        // Action
        var guidGenerator = await depository.ResolveDependencyAsync(typeof(IGuidGenerator));

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
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once, ImplementType: typeof(EmptyGuidGenerator),
                DefaultImplementation: null));
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once,
                ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));


        // Action
        var guidGenerator = await depository.ResolveDependenciesAsync(typeof(IGuidGenerator));

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
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once, ImplementType: typeof(EmptyGuidGenerator),
                DefaultImplementation: null));
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once,
                ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: null));


        // Action
        var guidGenerator = await depository.ResolveDependencyAsync(typeof(IEnumerable<IGuidGenerator>));

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
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        var emptyGuidGenerator = new EmptyGuidGenerator();
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once, ImplementType: typeof(EmptyGuidGenerator),
                DefaultImplementation: emptyGuidGenerator));

        // Action
        var resolveGuidGenerator = await depository.ResolveDependencyAsync(typeof(IGuidGenerator));

        // Assert
        resolveGuidGenerator.Should().Be(emptyGuidGenerator);
    }

    [Fact]
    public async void ResolveMultipleService_ToDefaultImplements_UsingResolves_ShouldAllReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        var emptyGuidGenerator = new EmptyGuidGenerator();
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once, ImplementType: typeof(EmptyGuidGenerator),
                DefaultImplementation: emptyGuidGenerator));

        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once,
                ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: emptyGuidGenerator));

        // Action
        var resolveGuidGenerator = await depository.ResolveDependenciesAsync(typeof(IGuidGenerator));

        // Assert
        resolveGuidGenerator.Should().AllSatisfy(item => item.Should().Be(emptyGuidGenerator));
    }

    [Fact]
    public async void ResolveMultipleService_ToDefaultImplement_UsingIEnumerable_ShouldAllReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var description = new DependencyDescription(DependencyType: typeof(IGuidGenerator),
            ResolvePolicy: DependencyResolvePolicy.LastWin, Lifetime: DependencyLifetime.Singleton);
        await depository.AddDependencyAsync(description);
        var emptyGuidGenerator = new EmptyGuidGenerator();
        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once, ImplementType: typeof(EmptyGuidGenerator),
                DefaultImplementation: emptyGuidGenerator));

        await depository.AddRelationAsync(description,
            new DependencyRelation(RelationType: DependencyRelationType.Once,
                ImplementType: typeof(RandomGuidGenerator), DefaultImplementation: emptyGuidGenerator));

        // Action
        var resolveGuidGenerator = await depository.ResolveDependencyAsync(typeof(IEnumerable<IGuidGenerator>));

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
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();

        // Action
        var guidGenerator = await depository.ResolveAsync<IGuidGenerator>();

        // Assert
        guidGenerator.Should().NotBeNull()
            .And.BeAssignableTo<IGuidGenerator>();
    }

    [Fact]
    public async void ResolveMultipleRegisteredService_ToSingleResolve_UsingExtension_ShouldReturnRandomGuidGenerator()
    {
        // Arrange
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();

        // Action
        var guidGenerator = await depository.ResolveAsync<IGuidGenerator>();

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
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();

        // Action
        var guidGenerator = await depository.ResolveMultipleAsync<IGuidGenerator>();

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
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();

        // Action
        var guidGenerator = await depository.ResolveAsync<IEnumerable<IGuidGenerator>>();

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
        await depository.AddSingletonAsync<ITypeGeneric<string>, TypeGeneric<string>>();

        // Action
        var guidGenerator = await depository.ResolveAsync<ITypeGeneric<string>>();

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
        await depository.AddSingletonAsync(typeof(ITypeGeneric<>), typeof(TypeGeneric<>));

        // Action
        var guidGenerator = await depository.ResolveAsync<ITypeGeneric<string>>();

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
        await depository.AddSingletonAsync(typeof(IConstructorInjectService), typeof(ConstructorInjectService));
        await depository.AddSingletonAsync(typeof(IGuidGenerator), typeof(RandomGuidGenerator));

        // Action
        var service = await depository.ResolveAsync<IConstructorInjectService>();

        // Assert
        service.As<ICheckIsNormal>().IsNormal.Should().BeTrue();
    }

    [Fact]
    public async void ResolveIEnumerableConstructorInject_ShouldBeNormal()
    {
        // Arrange
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync(typeof(IConstructorInjectService),
            typeof(ConstructorIEnumerableInjectService));
        await depository.AddSingletonAsync(typeof(IGuidGenerator), typeof(RandomGuidGenerator));
        await depository.AddSingletonAsync(typeof(IGuidGenerator), typeof(EmptyGuidGenerator));

        // Action
        var service = await depository.ResolveAsync<IConstructorInjectService>();

        // Assert
        service.As<ICheckIsNormal>().IsNormal.Should().BeTrue();
    }

    [Fact]
    public async void ResolveConstructorNotification_ShouldBeNormal()
    {
        // Arrange
        var depository = CreateNewDepository(option => option.AutoNotifyDependencyChange = true);
        await depository.AddSingletonAsync(typeof(IConstructorInjectService),
            typeof(ConstructorInjectNotifiableService));
        await depository.AddSingletonAsync(typeof(INotifyDependencyChanged<IGuidGenerator>),
            typeof(ConstructorInjectNotifiableService));
        await depository.AddSingletonAsync(typeof(IGuidGenerator), typeof(RandomGuidGenerator));


        // Action
        var service = await depository.ResolveAsync<IConstructorInjectService>();
        await depository.ChangeResolveTargetAsync(typeof(IGuidGenerator), new EmptyGuidGenerator());


        // Assert
        service.As<ICheckIsNormal>().IsNormal.Should().BeTrue();
    }

    [Fact]
    public async void ResolveIEnumerableConstructorNotification_ShouldBeNormal()
    {
        // Arrange
        var depository = CreateNewDepository(option => option.AutoNotifyDependencyChange = true);
        await depository.AddSingletonAsync(typeof(IConstructorInjectService),
            typeof(ConstructorInjectNotifiableService));
        await depository.AddSingletonAsync(typeof(INotifyDependencyChanged<IGuidGenerator>),
            typeof(ConstructorInjectNotifiableService));
        await depository.AddSingletonAsync(typeof(IGuidGenerator), typeof(RandomGuidGenerator));
        await depository.AddSingletonAsync(typeof(IGuidGenerator), typeof(EmptyGuidGenerator));


        // Action
        var service = await depository.ResolveAsync<IConstructorInjectService>();
        await depository.ChangeResolveTargetAsync(typeof(IGuidGenerator), new EmptyGuidGenerator());


        // Assert
        service.As<ICheckIsNormal>().IsNormal.Should().BeTrue();
    }

    [Fact]
    public async void ResolveService_ToDefaultImplement_UsingExtension_ShouldReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var emptyGuidGenerator = new EmptyGuidGenerator();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator);

        // Action
        var resolveGuidGenerator = await depository.ResolveAsync<IGuidGenerator>();

        // Assert
        resolveGuidGenerator.Should().Be(emptyGuidGenerator);
    }

    [Fact]
    public async void ResolveMultipleService_ToDefaultImplement_UsingExtension_ShouldAllReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var emptyGuidGenerator = new EmptyGuidGenerator();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator);
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>(emptyGuidGenerator);

        // Action
        var resolveGuidGenerator = await depository.ResolveMultipleAsync<IGuidGenerator>();

        // Assert
        resolveGuidGenerator.Should().AllSatisfy(item => item.Should().Be(emptyGuidGenerator));
    }
    
    [Fact]
    public async void ResolveMultipleService_ToDefaultImplement_UsingExtension_IEnumerable_ShouldAllReturnDefaultImplement()
    {
        // Arrange
        var depository = CreateNewDepository();
        var emptyGuidGenerator = new EmptyGuidGenerator();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>(emptyGuidGenerator);
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>(emptyGuidGenerator);

        // Action
        var resolveGuidGenerator = await depository.ResolveAsync<IEnumerable<IGuidGenerator>>();

        // Assert
        resolveGuidGenerator.Should().AllSatisfy(item => item.Should().Be(emptyGuidGenerator));
    }

    // Actions
    private static Core.Depository CreateNewDepository(Action<DepositoryOption>? options = null) => DepositoryFactory.CreateNew(options);
}