using Depository.Extensions.DependencyInjection;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Depository.Tests;

public class DependencyInjectionSupport
{

    private readonly HostApplicationBuilder _host;
    
    public DependencyInjectionSupport()
    {
        _host = Host.CreateApplicationBuilder();
        _host.ConfigureContainer(new DepositoryServiceProviderFactory());
        
    }
    
    [Fact]
    public void ResolveGuidGenerator_ShouldBeRandom()
    {
        // Init
        _host.Services.AddSingleton<IGuidGenerator, RandomGuidGenerator>();
        var app = _host.Build();
        
        // Action
        var guidGenerator = app.Services.GetRequiredService<IGuidGenerator>();
        var guid1 = guidGenerator.GetGuid();
        var guid2 = guidGenerator.GetGuid();
        
        // Assert
        guid1.Should().NotBe(guid2);
    }
}