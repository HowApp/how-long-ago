namespace How.Core.Infrastructure.Background.BackgroundTaskQueue;

using Microsoft.Extensions.DependencyInjection;

public interface IBackgroundTaskQueue
{
    void QueueBackgroundWorkItem(Func<IServiceScope, CancellationToken, Task> workItem);
    Task<Func<IServiceScope, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}