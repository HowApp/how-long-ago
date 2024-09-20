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

    private const int MaxDegreeParallelism = 5;

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

        var tasks = new List<Task>();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            while (tasks.Count < MaxDegreeParallelism)
            {
                var workItem = await _backgroundTaskQueue.DequeueAsync(cancellationToken);

                if (workItem is not null)
                {
                    var task = Task.Run(async () =>
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
                    }, cancellationToken);
                    
                    tasks.Add(task);
                }
            }
            
            tasks.Remove(await Task.WhenAny(tasks));
        }
        
        _logger.LogInformation("QueueHostedService is stopping.");
    }
}