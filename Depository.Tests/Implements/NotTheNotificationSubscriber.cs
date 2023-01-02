using Depository.Abstraction.Interfaces;
using Depository.Demo.Interfaces;

namespace Depository.Demo.Implements;

public class NotTheNotificationSubscriber : INotificationSubscriber<string>, ICheckIsNormal
{
    public Task HandleNotification(string notification)
    {
        IsNormal = true;
        return Task.CompletedTask;
    }

    public bool IsNormal { get; set; }
}