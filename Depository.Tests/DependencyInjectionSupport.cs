using Depository.Extensions.DependencyInjection;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


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
    public async Task ResolveGuidGenerator_ShouldBeRandom()
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
    public async Task ResolveGuidGenerator_InScope_ShouldBeRandom()
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
    public async Task ResolveKeyedGuidGenerator_ShouldNotBeSame()
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
    public async Task ResolveNamedGuidGenerator_ShouldNotBeSame()
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
}