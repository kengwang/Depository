namespace Depository.Abstraction.Interfaces;

public interface INotificationSubscriber<in TNotification>
{
    public Task HandleNotification(TNotification notification, CancellationToken ctk = new());
}

public interface INotificationSubscriber<in TNotification, TResult>
{
    public Task<TResult> HandleNotification(TNotification notification, CancellationToken ctk = new());
}