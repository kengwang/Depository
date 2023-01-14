using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models.Options;
using Depository.Core;
using Depository.Demo.Implements;
using Depository.Demo.Interfaces;
using Depository.Extensions;
using FluentAssertions;
using Xunit;

namespace Depository.Demo;

public class DepositoryNotificationTests
{

    [Fact]
    public async void PublishNotification_ToSingle_ShouldReceive()
    {
        // Init
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync<INotificationSubscriber<TestNotification>, NormalNotificationSubscriber>();
        
        // Action
        await depository.PublishNotificationAsync(new TestNotification());
        

        // Assert
        var receiver = await depository.ResolveAsync<INotificationSubscriber<TestNotification>>();
        receiver
            .As<NormalNotificationSubscriber>()
            .IsNormal.Should().BeTrue();
    }
    
   
    [Fact]
    public async void PublishNotification_ToMultiple_ShouldReceive()
    {
        // Init
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync<INotificationSubscriber<TestNotification>, NormalNotificationSubscriber>();
        await depository.AddSingletonAsync<INotificationSubscriber<TestNotification>, AnotherNotificationSubscriber>();
        await depository.AddSingletonAsync<INotificationSubscriber<string>, NotTheNotificationSubscriber>();
        
        // Action
        await depository.PublishNotificationAsync(new TestNotification());
        

        // Assert
        var receivers = await depository.ResolveMultipleAsync<INotificationSubscriber<TestNotification>>();
        receivers
            .Cast<ICheckIsNormal>()
            .Should().HaveCount(2)
            .And.AllSatisfy(t => t.IsNormal.Should().BeTrue());
    }
    
    [Fact]
    public async void PublishResultedNotification_ToSingle_ShouldReceive()
    {
        // Init
        var depository = CreateNewDepository();
        await depository.AddSingletonAsync<INotificationSubscriber<TestNotification, string>, ResultedNotificationSubscriber>();
        
        // Action
        var result = await depository.PublishNotificationWithResultAsync<TestNotification, string>(new TestNotification());
        

        // Assert
        var receiver = await depository.ResolveAsync<INotificationSubscriber<TestNotification, string>>();
        receiver
            .As<ICheckIsNormal>()
            .IsNormal.Should().BeTrue();
        result.Should().Contain("Received");
    }
    
    // Actions
    private static Core.Depository CreateNewDepository(Action<DepositoryOption>? options = null) => DepositoryFactory.CreateNew(options);
}