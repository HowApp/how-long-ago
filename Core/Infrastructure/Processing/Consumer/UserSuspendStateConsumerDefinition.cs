namespace How.Core.Infrastructure.Processing.Consumer;

using MassTransit;

public class UserSuspendStateConsumerDefinition : ConsumerDefinition<UserSuspendStateConsumer>
{
    public UserSuspendStateConsumerDefinition()
    {
        ConcurrentMessageLimit = 4;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<UserSuspendStateConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
    }
}