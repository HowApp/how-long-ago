namespace How.Core.Infrastructure.Processing.Consumer;

using MassTransit;

public class UserRegisterBulkConsumerDefinition : ConsumerDefinition<UserRegisterBulkConsumer>
{
    public UserRegisterBulkConsumerDefinition()
    {
        ConcurrentMessageLimit = 4;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<UserRegisterBulkConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
    }
}