namespace How.Core.Infrastructure.Processing.Consumer;

using CQRS.Commands.Internal.UserUpdateSuspend;
using HowCommon.MassTransitContract;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

public class UserSuspendStateConsumer : IConsumer<UserSuspendedStateMessage>
{
    private readonly ILogger<UserSuspendStateConsumer> _logger;
    private readonly ISender _sender;
    public UserSuspendStateConsumer(ILogger<UserSuspendStateConsumer> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<UserSuspendedStateMessage> context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", context.Message.UserId);

        if (context.Message.UserId == 0)
        {
            _logger.LogError("Received User Suspend State Message without User ID");
        }

        var result = await _sender.Send(new InternalUserUpdateSuspendCommand
        {
            UserId = context.Message.UserId,
            State = context.Message.IsSuspended,
        });

        if (result.Succeeded && result.Data == 0)
        {
            _logger.LogInformation($"User suspend state not updated. User ID: {context.Message.UserId}");
        }
        else
        {
            _logger.LogError(result.GetErrorMessages());
        }
    }
}