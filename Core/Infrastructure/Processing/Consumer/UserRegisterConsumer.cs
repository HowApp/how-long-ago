namespace How.Core.Infrastructure.Processing.Consumer;

using CQRS.Commands.Internal.UserRegister;
using HowCommon.MassTransitContract;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

public class UserRegisterConsumer : IConsumer<UserRegisterMessage>
{
    private readonly ILogger<UserRegisterConsumer> _logger;
    private readonly ISender _sender;

    public UserRegisterConsumer(ILogger<UserRegisterConsumer> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<UserRegisterMessage> context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", context.Message.UserId);

        if (context.Message.UserId == 0)
        {
            _logger.LogError("Received User Register Message without User ID");
        }
        
        var result = await _sender.Send(new UserRegisterCommand
        {
            UserId = context.Message.UserId,
        });

        if (result.Succeeded && result.Data == 0)
        {
            _logger.LogInformation($"User not registered. User ID: {context.Message.UserId}");
        }
        else
        {
            _logger.LogError(result.GetErrorMessages());
        }
    }
}