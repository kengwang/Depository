using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.NotificationHub;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Depository.Extensions;
using Depository.Tests.Implements;
using Depository.Tests.Interfaces;
using FluentAssertions;
using TUnit.Core;

namespace Depository.Tests;

public class DepositoryNotificationTests
{

    [Test]
    public async Task PublishNotification_ToSingle_ShouldReceive()
    {
        // Init
        var depository = CreateNewDepository();
        depository.AddSingleton<INotificationSubscriber<TestNotification>, NormalNotificationSubscriber>();
        
        // Action
        var hub = depository.Resolve<INotificationHub>();
        await hub.PublishNotificationAsync(new TestNotification());
        

        // Assert
        var receiver = depository.Resolve<INotificationSubscriber<TestNotification>>();
        receiver
            .As<NormalNotificationSubscriber>()
            .IsNormal.Should().BeTrue();
    }
    
   
    [Test]
    public async Task PublishNotification_ToMultiple_ShouldReceive()
    {
        // Init
        var depository = CreateNewDepository();
        depository.AddSingleton<INotificationSubscriber<TestNotification>, NormalNotificationSubscriber>();
        depository.AddSingleton<INotificationSubscriber<TestNotification>, AnotherNotificationSubscriber>();
        depository.AddSingleton<INotificationSubscriber<string>, NotTheNotificationSubscriber>();
        
        // Action
        var hub = depository.Resolve<INotificationHub>();
        await hub.PublishNotificationAsync(new TestNotification());
        

        // Assert
        var receivers = depository.ResolveMultiple<INotificationSubscriber<TestNotification>>();
        receivers
            .Cast<ICheckIsNormal>()
            .Should().HaveCount(2)
            .And.AllSatisfy(t => t.IsNormal.Should().BeTrue());
    }
    
    [Test]
    public async Task PublishResultedNotification_ToSingle_ShouldReceive()
    {
        // Init
        var depository = CreateNewDepository();
        depository.AddSingleton<INotificationSubscriber<TestNotification, string>, ResultedNotificationSubscriber>();
        
        // Action
        var hub = depository.Resolve<INotificationHub>();
        var result = await hub.PublishNotificationWithResultAsync<TestNotification, string>(new TestNotification());
        

        // Assert
        var receiver = depository.Resolve<INotificationSubscriber<TestNotification, string>>();
        receiver
            .As<ICheckIsNormal>()
            .IsNormal.Should().BeTrue();
        result.Should().Contain("Received");
    }
    
    // Actions
    private static Core.Depository CreateNewDepository(Action<DepositoryOption>? options = null) => DepositoryFactory.CreateNew(options);
}
