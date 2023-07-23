using Depository.Abstraction.Interfaces;
using Depository.Tests.Interfaces;

namespace Depository.Tests.Implements;

public class NotTheNotificationSubscriber : INotificationSubscriber<string>, ICheckIsNormal
{
    public Task HandleNotificationAsync(string notification, CancellationToken ctk = new())
    {
        IsNormal = true;
        return Task.CompletedTask;
    }

    public bool IsNormal { get; set; }
}