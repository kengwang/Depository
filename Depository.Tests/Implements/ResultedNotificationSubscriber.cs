using Depository.Abstraction.Interfaces;
using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class ResultedNotificationSubscriber : INotificationSubscriber<TestNotification, string>, ICheckIsNormal
{

   
    public Task<string> HandleNotification(TestNotification notification)
    {
        IsNormal = true;
        return Task.FromResult("Received");
    }

    public bool IsNormal { get; set; }
}