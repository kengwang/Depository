using BenchmarkDotNet.Attributes;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.NotificationHub;
using Depository.Benchmarks.Implements;
using Depository.Benchmarks.Interfaces;
using Depository.Core;
using Depository.Extensions;

// ReSharper disable once CheckNamespace
namespace Depository.Benchmarks;

public partial class Benchmarks
{
    [Benchmark]
    public async Task PublishNotification()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<INotificationSubscriber<Notification>, NotificationSubscriber>();
        var hub = depository.Resolve<INotificationHub>();
        await hub.PublishNotificationAsync(new Notification());
    }
    
    [Benchmark]
    public async Task PublishNotificationWithResult()
    {
        var depository = DepositoryFactory.CreateNew();
        depository.AddSingleton<INotificationSubscriber<Notification,NotificationResult>, NotificationAndResultSubscriber>();
        var hub = depository.Resolve<INotificationHub>();
        await hub.PublishNotificationWithResultAsync<Notification,NotificationResult>(new Notification());
    }
}


// ReSharper disable once ClassNeverInstantiated.Global
public class Notification
{
    
}

// ReSharper disable once ClassNeverInstantiated.Global
public class NotificationResult
{
    
}

// ReSharper disable once ClassNeverInstantiated.Global
public class NotificationSubscriber : INotificationSubscriber<Notification>
{
    public Task HandleNotificationAsync(Notification notification, CancellationToken ctk = new CancellationToken())
    {
        return Task.CompletedTask;
    }
}

public class NotificationAndResultSubscriber : INotificationSubscriber<Notification, NotificationResult>
{

    public Task<NotificationResult> HandleNotificationAsync(Notification notification, CancellationToken ctk = new CancellationToken())
    {
        return Task.FromResult(new NotificationResult());
    }
}