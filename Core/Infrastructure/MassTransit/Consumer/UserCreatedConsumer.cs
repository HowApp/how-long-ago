namespace How.Core.Infrastructure.MassTransit.Consumer;

using Common.MassTransitContracts.Consumer;
using global::MassTransit;
using Microsoft.Extensions.Logging;

public class UserCreatedConsumer : IConsumer<UserRegisterMessage>
{
    readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UserRegisterMessage> context)
    {
        _logger.LogInformation("Content Received: {UserId}", context.Message.UserId);

        return Task.CompletedTask;
    }
}