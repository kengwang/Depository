using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.NotificationHub;
using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class ResultedNotificationSubscriber : INotificationSubscriber<TestNotification, string>, ICheckIsNormal
{

   
    public Task<string> HandleNotificationAsync(TestNotification notification, CancellationToken ctk = new())
    {
        IsNormal = true;
        return Task.FromResult("Received");
    }

    public bool IsNormal { get; set; }
}