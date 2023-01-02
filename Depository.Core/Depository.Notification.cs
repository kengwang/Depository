using Depository.Abstraction.Interfaces;

namespace Depository.Core;

public partial class Depository
{
    public async Task PublishNotificationAsync<TNotification>(TNotification notification)
    {
        var subscribers =
            (await ResolveDependenciesAsync(typeof(INotificationSubscriber<TNotification>)))
            .Select(receiver => (INotificationSubscriber<TNotification>)receiver)
            .ToList();
        foreach (var subscriber in subscribers)
        {
            try
            {
                await subscriber.HandleNotification(notification);
            }
            catch
            {
                // ignored
            }
        }
    }

    public async Task<List<TResult>> PublishNotificationWithResultAsync<TNotification, TResult>(
        TNotification notification)
    {
        var subscribers =
            (await ResolveDependenciesAsync(typeof(INotificationSubscriber<TNotification, TResult>)))
            .Select(receiver => (INotificationSubscriber<TNotification, TResult>)receiver)
            .ToList();
        var results = new List<TResult>();
        foreach (var subscriber in subscribers)
        {
            try
            {
                results.Add(await subscriber.HandleNotification(notification));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return results;
    }
}