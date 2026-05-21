using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.NotificationHub;

namespace Depository.Core;

public class NotificationHub : INotificationHub
{

    private readonly IDepository _depository;
    
    public NotificationHub(IDepository depository)
    {
        _depository = depository;
    }
    
    public async Task PublishNotificationAsync<TNotification>(TNotification notification, CancellationToken ctk = new())
    {
        var subscribers =
            (_depository.ResolveDependencies(typeof(INotificationSubscriber<TNotification>)))
            .Select(receiver => (INotificationSubscriber<TNotification>)receiver)
            .ToList();
        var tasks = subscribers
                    .Select(handler => handler.HandleNotificationAsync(notification, ctk))
                    .ToArray();
        await Task.WhenAll(tasks);
    }

    public async Task<List<TResult>> PublishNotificationWithResultAsync<TNotification, TResult>(
        TNotification notification,CancellationToken ctk = new())
    {
        var subscribers =
            (_depository.ResolveDependencies(typeof(INotificationSubscriber<TNotification, TResult>)))
            .Select(receiver => (INotificationSubscriber<TNotification, TResult>)receiver)
            .ToList();
        var results = new List<TResult>();
        foreach (var subscriber in subscribers)
        {
            try
            {
                results.Add(await subscriber.HandleNotificationAsync(notification, ctk).ConfigureAwait(false));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return results;
    }
}