using Depository.Abstraction.Interfaces;
using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class AnotherNotificationSubscriber : INotificationSubscriber<TestNotification>, ICheckIsNormal
{
    public bool IsNormal { get; set; }
    public Task HandleNotificationAsync(TestNotification notification, CancellationToken ctk = new CancellationToken())
    {
        IsNormal = true;
        return Task.CompletedTask;
    }
}