﻿namespace Depository.Abstraction.Interfaces;

public interface INotificationHub
{
    public Task PublishNotificationAsync<TNotification>(TNotification notification, CancellationToken ctk = new());
    public Task<List<TResult>> PublishNotificationWithResultAsync<TNotification, TResult>(TNotification notification, CancellationToken ctk = new());
}