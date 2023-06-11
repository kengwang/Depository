using Depository.Abstraction.Interfaces;
using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class ResultedNotificationSubscriber : INotificationSubscriber<TestNotification, string>, ICheckIsNormal
{

   
    public Task<string> HandleNotification(TestNotification notification, CancellationToken ctk = new())
    {
        IsNormal = true;
        return Task.FromResult("Received");
    }

    public bool IsNormal { get; set; }
}