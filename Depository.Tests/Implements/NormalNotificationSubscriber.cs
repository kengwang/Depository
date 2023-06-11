using Depository.Abstraction.Interfaces;
using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class NormalNotificationSubscriber : INotificationSubscriber<TestNotification>, ICheckIsNormal
{

   
    public Task HandleNotification(TestNotification notification, CancellationToken ctk = new())
    {
        IsNormal = true;
        return Task.CompletedTask;
    }

    public bool IsNormal { get; set; }
}

public class TestNotification
{
    
}