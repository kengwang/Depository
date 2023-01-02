using Depository.Abstraction.Interfaces;
using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class NormalNotificationSubscriber : INotificationSubscriber<TestNotification>, ICheckIsNormal
{

   
    public Task HandleNotification(TestNotification notification)
    {
        IsNormal = true;
        return Task.CompletedTask;
    }

    public bool IsNormal { get; set; }
}

public class TestNotification
{
    
}