namespace How.Core.Infrastructure.Processing.Consumer;

using CQRS.Commands.Internal.UserDelete;
using HowCommon.MassTransitContract;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

public class UserDeletedConsumer : IConsumer<UserDeletedMessage>
{
    private readonly ILogger<UserDeletedConsumer> _logger;
    private readonly ISender _sender;

    public UserDeletedConsumer(ILogger<UserDeletedConsumer> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }


    public async Task Consume(ConsumeContext<UserDeletedMessage> context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", context.Message.UserId);

        if (context.Message.UserId == 0)
        {
            _logger.LogError("Received User Deleted Message without User ID");
        }
        
        var salt = "Deleted_" + Guid.NewGuid();
        
        var result = await _sender.Send(new InternalUserDeleteCommand
        {
            UserId = context.Message.UserId,
            Salt = salt
        });
        
        if (result.Succeeded && result.Data == 0)
        {
            _logger.LogInformation($"User not deleted. User ID: {context.Message.UserId}");
        }
        else
        {
            _logger.LogError(result.GetErrorMessages());
        }
    }
}