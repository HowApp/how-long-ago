namespace How.Core.Infrastructure.Processing.Producer;

using HowCommon.Infrastructure.Helpers;
using MassTransit;
using Microsoft.Extensions.Logging;

public class UserServiceAccountProducer
{
    private readonly ILogger<UserServiceAccountProducer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly UserIdHelper _helper;

    public UserServiceAccountProducer(
        ILogger<UserServiceAccountProducer> logger,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _helper = new UserIdHelper((sender, message) => logger.LogError(message));
    }

    private async Task PublishMessageAsync<T>(T message) where T : class
    {
        if (message == null)
        {
            _logger.LogError("Attempted to publish a null message of type {MessageType}", typeof(T).Name);
            return;
        }

        try
        {
            await _publishEndpoint.Publish(message);
            _logger.LogInformation("Published message of type {MessageType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message of type {MessageType}", typeof(T).Name);
        }
    }
}