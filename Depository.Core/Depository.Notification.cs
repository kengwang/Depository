using Depository.Abstraction.Interfaces;

namespace Depository.Core;

public partial class Depository
{
    public async Task PublishNotificationAsync<TNotification>(TNotification notification)
    {
        var subscribers =
            (await ResolveDependenciesAsync(typeof(INotificationSubscriber<TNotification>)))
            .Cast<INotificationSubscriber<TNotification>>()
            .ToList();
        foreach (var subscriber in subscribers)
        {
            await subscriber.HandleNotification(notification);
        }
    }

    public async Task<List<TResult>> PublishNotificationWithResultAsync<TNotification, TResult>(
        TNotification notification)
    {
        var subscribers =
            (await ResolveDependenciesAsync(typeof(INotificationSubscriber<TNotification, TResult>)))
            .Cast<INotificationSubscriber<TNotification, TResult>>()
            .ToList();
        var results = new List<TResult>();
        foreach (var subscriber in subscribers)
        {
            results.Add(await subscriber.HandleNotification(notification));
        }

        return results;
    }
}