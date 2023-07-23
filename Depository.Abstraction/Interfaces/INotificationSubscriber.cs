namespace Depository.Abstraction.Interfaces;

public interface INotificationSubscriber<in TNotification>
{
    public Task HandleNotificationAsync(TNotification notification, CancellationToken ctk = new());
}

public interface INotificationSubscriber<in TNotification, TResult>
{
    public Task<TResult> HandleNotificationAsync(TNotification notification, CancellationToken ctk = new());
}