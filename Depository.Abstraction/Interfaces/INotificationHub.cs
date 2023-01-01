namespace Depository.Abstraction.Interfaces;

public interface INotificationHub
{
    public Task PublishNotificationAsync<TNotification>(TNotification notification);
    public Task<List<TResult>> PublishNotificationWithResultAsync<TNotification, TResult>(TNotification notification);
}