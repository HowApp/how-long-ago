namespace How.Core.Infrastructure.Workers.Consumer;

using MassTransit;

public class UserRegisterConsumerDefinition : ConsumerDefinition<UserRegisterConsumer>
{
    public UserRegisterConsumerDefinition()
    {
        ConcurrentMessageLimit = 4;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<UserRegisterConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
    }
}