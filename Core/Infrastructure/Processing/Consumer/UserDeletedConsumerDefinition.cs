namespace How.Core.Infrastructure.Processing.Consumer;

using MassTransit;

public class UserDeletedConsumerDefinition : ConsumerDefinition<UserDeletedConsumer>
{
    public UserDeletedConsumerDefinition()
    {
        ConcurrentMessageLimit = 4;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<UserDeletedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
    }
}