using Depository.Abstraction.Enums;
using Depository.Abstraction.Models;
using Depository.Demo.Implements;
using Depository.Demo.Interfaces;
using Depository.Extensions;
using FluentAssertions;
using Xunit;

namespace Depository.Demo;

public class DepositoryResolveTests
{
    // Pure

    [Fact]
    public async void ResolveSingleRegisteredService_ToSingleResolve_ShouldReturnEmptyGuidGenerator()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription
        {
            DependencyType = typeof(IGuidGenerator),
            ResolvePolicy = DependencyResolvePolicy.LastWin,
            Lifetime = DependencyLifetime.Singleton
        };
        await depository.AddDependencyAsync(description);
        await depository.AddRelationAsync(description, new DependencyRelation
        {
            RelationType = DependencyRelationType.Once,
            ImplementType = typeof(RandomGuidGenerator),
            DefaultImplementation = null
        });


        // Action
        var guidGenerator = await depository.ResolveDependencyAsync(typeof(IGuidGenerator));

        // Assert
        guidGenerator.Should().NotBeNull().And.BeOfType<RandomGuidGenerator>();
    }

    [Fact]
    public async void ResolveMultipleRegisteredService_ToSingleResolve_ShouldReturnEmptyGuidGenerator()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription
        {
            DependencyType = typeof(IGuidGenerator),
            ResolvePolicy = DependencyResolvePolicy.LastWin,
            Lifetime = DependencyLifetime.Singleton
        };
        await depository.AddDependencyAsync(description);
        await depository.AddRelationAsync(description, new DependencyRelation
        {
            RelationType = DependencyRelationType.Once,
            ImplementType = typeof(EmptyGuidGenerator),
            DefaultImplementation = null
        });
        await depository.AddRelationAsync(description, new DependencyRelation
        {
            RelationType = DependencyRelationType.Once,
            ImplementType = typeof(RandomGuidGenerator),
            DefaultImplementation = null
        });


        // Action
        var guidGenerator = await depository.ResolveDependencyAsync(typeof(IGuidGenerator));

        // Assert
        guidGenerator.Should().NotBeNull().And.BeOfType<RandomGuidGenerator>();
    }


    [Fact]
    public async void ResolveMultipleRegisteredService_ToMultipleResolves_ShouldReturnMultipleGuidGenerators()
    {
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription
        {
            DependencyType = typeof(IGuidGenerator),
            ResolvePolicy = DependencyResolvePolicy.LastWin,
            Lifetime = DependencyLifetime.Singleton
        };
        await depository.AddDependencyAsync(description);
        await depository.AddRelationAsync(description, new DependencyRelation
        {
            RelationType = DependencyRelationType.Once,
            ImplementType = typeof(EmptyGuidGenerator),
            DefaultImplementation = null
        });
        await depository.AddRelationAsync(description, new DependencyRelation
        {
            RelationType = DependencyRelationType.Once,
            ImplementType = typeof(RandomGuidGenerator),
            DefaultImplementation = null
        });


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
        // Init
        var depository = CreateNewDepository();
        var description = new DependencyDescription
        {
            DependencyType = typeof(IGuidGenerator),
            ResolvePolicy = DependencyResolvePolicy.LastWin,
            Lifetime = DependencyLifetime.Singleton
        };
        await depository.AddDependencyAsync(description);
        await depository.AddRelationAsync(description, new DependencyRelation
        {
            RelationType = DependencyRelationType.Once,
            ImplementType = typeof(EmptyGuidGenerator),
            DefaultImplementation = null
        });
        await depository.AddRelationAsync(description, new DependencyRelation
        {
            RelationType = DependencyRelationType.Once,
            ImplementType = typeof(RandomGuidGenerator),
            DefaultImplementation = null
        });


        // Action
        var guidGenerator = await depository.ResolveDependencyAsync(typeof(IEnumerable<IGuidGenerator>));

        // Assert
        var guidGeneratorList = Assert.IsAssignableFrom<IEnumerable<IGuidGenerator>>(guidGenerator).ToList();
        guidGeneratorList.Should().NotBeEmpty()
            .And.ContainItemsAssignableTo<IGuidGenerator>()
            .And.HaveCount(2);
    }

    // Extensions

    [Fact]
    public async void ResolveSingleRegisteredService_ToSingleResolve_UsingExtension_ShouldReturnEmptyGuidGenerator()
    {
        // Init
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();

        // Action
        var guidGenerator = await depository.ResolveAsync<IGuidGenerator>();

        // Assert
        guidGenerator.Should().NotBeNull()
            .And.BeAssignableTo<IGuidGenerator>();
    }

    [Fact]
    public async void ResolveMultipleRegisteredService_ToSingleResolve_UsingExtension_ShouldReturnEmptyGuidGenerator()
    {
        // Init
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync<IGuidGenerator, EmptyGuidGenerator>();
        await depository.AddSingletonAsync<IGuidGenerator, RandomGuidGenerator>();

        // Action
        var guidGenerator = await depository.ResolveAsync<IGuidGenerator>();

        // Assert
        guidGenerator.Should().NotBeNull()
            .And.BeAssignableTo<IGuidGenerator>();
    }

    [Fact]
    public async void
        ResolveMultipleRegisteredService_ToMultipleResolves_UsingExtension_ShouldReturnMultipleGuidGenerators()
    {
        // Init
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
        // Init
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

    // Actions
    private Core.Depository CreateNewDepository() => new();
}