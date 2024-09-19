namespace How.Core.Infrastructure.Background.Workers;

using BackgroundTaskQueue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class QueueHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly ILogger<QueueHostedService> _logger;

    public QueueHostedService(IBackgroundTaskQueue backgroundTaskQueue, ILogger<QueueHostedService> logger)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("QueueHostedService is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var workItem = await _backgroundTaskQueue.DequeueAsync(cancellationToken);

            try
            {
                await workItem(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred executing background work item.");
            }
        }
        
        _logger.LogInformation("QueueHostedService is stoping.");
    }
}