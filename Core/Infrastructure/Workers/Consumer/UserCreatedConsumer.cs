namespace How.Core.Infrastructure.Workers.Consumer;

using Common.MassTransitContracts.Consumer;
using MassTransit;
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

public class UserCreatedConsumerDefinition : ConsumerDefinition<UserCreatedConsumer>
{
   protected override void ConfigureConsumer(
       IReceiveEndpointConfigurator endpointConfigurator,
       IConsumerConfigurator<UserCreatedConsumer> consumerConfigurator,
       IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
    }
}