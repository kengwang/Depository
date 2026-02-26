using System.Diagnostics.CodeAnalysis;

namespace Depository.Abstraction.Interfaces;

public interface INotificationHub
{
    [RequiresDynamicCode("Open-generic type resolution uses MakeGenericType at runtime.")]
    public Task PublishNotificationAsync<TNotification>(TNotification notification, CancellationToken ctk = new());

    [RequiresDynamicCode("Open-generic type resolution uses MakeGenericType at runtime.")]
    public Task<List<TResult>> PublishNotificationWithResultAsync<TNotification, TResult>(TNotification notification, CancellationToken ctk = new());
}