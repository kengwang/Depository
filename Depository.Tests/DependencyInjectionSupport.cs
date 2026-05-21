using System.Collections.Specialized;
using Depository.Extensions.DependencyInjection;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TUnit.Core;

namespace Depository.Tests;

public class DependencyInjectionSupport
{
    private readonly HostApplicationBuilder _host;

    public DependencyInjectionSupport()
    {
        _host = Host.CreateApplicationBuilder();
        _host.ConfigureContainer(new DepositoryServiceProviderFactory());
    }

    [Test]
    public void ResolveGuidGenerator_ShouldBeRandom()
    {
        // Init
        _host.Services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        var app = _host.Build();

        // Action
        var guidGenerator1 = app.Services.GetRequiredService<IGuidGenerator>();
        var guidGenerator2 = app.Services.GetRequiredService<IGuidGenerator>();
        var guid1 = guidGenerator1.GetGuid();
        var guid2 = guidGenerator2.GetGuid();

        // Assert
        guid1.Should().Be(guid2);
    }

    [Test]
    public void ResolveGuidGenerator_InScope_ShouldBeRandom()
    {
        // Init
        _host.Services.AddScoped<IGuidGenerator, RandomGuidGenerator>();
        var app = _host.Build();

        // Action
        var guidGenerator = app.Services.CreateScope().ServiceProvider.GetRequiredService<IGuidGenerator>();
        var guid1 = guidGenerator.GetGuid();
        var guid2 = guidGenerator.GetGuid();

        // Assert
        guid1.Should().Be(guid2);
    }

    [Test]
    public void ResolveKeyedGuidGenerator_ShouldNotBeSame()
    {
        // Arrange
        _host.Services.AddKeyedSingleton<IGuidGenerator, RandomGuidGenerator>("a");
        _host.Services.AddKeyedSingleton<IGuidGenerator, RandomGuidGenerator>("a");
        _host.Services.AddKeyedSingleton<IGuidGenerator, RandomGuidGenerator>("b");
        
        var app = _host.Build();
        
        // Action
        var guidGeneratorA = app.Services.GetKeyedService<IEnumerable<IGuidGenerator>>("a")?.ToList();
        var guidGeneratorB = app.Services.GetKeyedService<IGuidGenerator>("b");
        
        // Assert
        guidGeneratorA.Should().HaveCount(2);
        guidGeneratorA.Should().AllSatisfy(t=>t.Should().NotBeSameAs(guidGeneratorB));

    }

    [Test]
    public void ResolveKeyedEnumerable_ShouldReturnOnlyMatchingKeyedServices()
    {
        _host.Services.AddKeyedSingleton<IGuidGenerator, RandomGuidGenerator>("a");
        _host.Services.AddKeyedSingleton<IGuidGenerator, EmptyGuidGenerator>("b");

        var app = _host.Build();

        var guidGenerators = app.Services.GetKeyedService<IEnumerable<IGuidGenerator>>("a")?.ToList();

        guidGenerators.Should().ContainSingle();
        guidGenerators![0].Should().BeOfType<RandomGuidGenerator>();
    }
    
    [Test]
    public void ResolveNamedGuidGenerator_ShouldNotBeSame()
    {
        // Arrange
        
        var randomGuidGeneratorA = new RandomGuidGenerator();
        var randomGuidGeneratorB = new RandomGuidGenerator();
        
        _host.Services.AddKeyedSingleton<IGuidGenerator, RandomGuidGenerator>("a", (_, _) => randomGuidGeneratorA );
        _host.Services.AddKeyedSingleton<IGuidGenerator, RandomGuidGenerator>("b", (_, _) => randomGuidGeneratorB);
        _host.Services.AddSingleton<ConstructorFromKeyedService>();
        var app = _host.Build();
        
        // Action
        var service = app.Services.GetRequiredService<ConstructorFromKeyedService>();
        
        // Assert
        service.GuidGenerator.Should().NotBeNull();
        service.GuidGenerator.Should().Be(randomGuidGeneratorB);
    }

    [Test]
    public void ResolveFactoryGuidGenerator_ShouldUseRegisteredFactory()
    {
        var randomGuidGenerator = new RandomGuidGenerator();
        _host.Services.AddSingleton<IGuidGenerator>(_ => randomGuidGenerator);
        var app = _host.Build();

        var resolvedGenerator = app.Services.GetRequiredService<IGuidGenerator>();

        resolvedGenerator.Should().BeSameAs(randomGuidGenerator);
    }

    [Test]
    public void ResolveInstanceGuidGenerator_ShouldUseRegisteredInstance()
    {
        var randomGuidGenerator = new RandomGuidGenerator();
        _host.Services.AddSingleton<IGuidGenerator>(randomGuidGenerator);

        var app = _host.Build();

        var resolvedGenerator = app.Services.GetRequiredService<IGuidGenerator>();

        resolvedGenerator.Should().BeSameAs(randomGuidGenerator);
    }

    [Test]
    public void ResolveKeyedInstanceGuidGenerator_ShouldUseRegisteredInstance()
    {
        var randomGuidGenerator = new RandomGuidGenerator();
        _host.Services.AddKeyedSingleton<IGuidGenerator>("a", randomGuidGenerator);

        var app = _host.Build();

        var resolvedGenerator = app.Services.GetRequiredKeyedService<IGuidGenerator>("a");

        resolvedGenerator.Should().BeSameAs(randomGuidGenerator);
    }

    [Test]
    public void GetOptionalService_WhenMissing_ShouldReturnNull()
    {
        var app = _host.Build();

        var service = app.Services.GetService<IConstructorInjectService>();

        service.Should().BeNull();
    }

    [Test]
    public void ServiceProviderIsService_ShouldReportRegisteredServices()
    {
        _host.Services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        var app = _host.Build();

        var serviceChecker = app.Services.GetRequiredService<IServiceProviderIsService>();

        serviceChecker.IsService(typeof(IGuidGenerator)).Should().BeTrue();
        serviceChecker.IsService(typeof(IConstructorInjectService)).Should().BeFalse();
    }

    [Test]
    public void ServiceProviderIsKeyedService_ShouldReportRegisteredKeyedServices()
    {
        _host.Services.AddKeyedSingleton<IGuidGenerator, RandomGuidGenerator>("known");
        var app = _host.Build();

        var serviceChecker = app.Services.GetRequiredService<IServiceProviderIsKeyedService>();

        serviceChecker.IsKeyedService(typeof(IGuidGenerator), "known").Should().BeTrue();
        serviceChecker.IsKeyedService(typeof(IGuidGenerator), "missing").Should().BeFalse();
    }
}
