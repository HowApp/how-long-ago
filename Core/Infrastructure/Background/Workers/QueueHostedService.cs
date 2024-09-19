namespace How.Core.Infrastructure.Background.Workers;

using BackgroundTaskQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class QueueHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<QueueHostedService> _logger;

    public QueueHostedService(
        IBackgroundTaskQueue backgroundTaskQueue,
        ILogger<QueueHostedService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("QueueHostedService is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var workItem = await _backgroundTaskQueue.DequeueAsync(cancellationToken);

            if (workItem is not null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        await workItem(scope, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error occurred executing background work item.");
                    }
                }
            }
            
        }
        
        _logger.LogInformation("QueueHostedService is stopping.");
    }
}