namespace Depository.Abstraction.Interfaces;

public interface INotificationSubscriber<in TNotification>
{
    public Task HandleNotification(TNotification notification);
}

public interface INotificationSubscriber<in TNotification, TResult>
{
    public Task<TResult> HandleNotification(TNotification notification);
}