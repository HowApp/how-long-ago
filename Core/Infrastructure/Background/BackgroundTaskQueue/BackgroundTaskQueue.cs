namespace How.Core.Infrastructure.Background.BackgroundTaskQueue;

using System.Collections.Concurrent;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private ConcurrentQueue<Func<CancellationToken, Task>> _queue = new();
    private SemaphoreSlim _semaphore = new(0);
    public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
    {
        if (_queue is null)
        {
            throw new ArgumentNullException(nameof(_queue));
        }
        
        _queue.Enqueue(workItem);
        _semaphore.Release();
    }

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        _queue.TryDequeue(out var workItem);
        return workItem;
    }
}