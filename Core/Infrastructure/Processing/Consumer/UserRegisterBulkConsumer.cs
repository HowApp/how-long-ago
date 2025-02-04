namespace How.Core.Infrastructure.Processing.Consumer;

using CQRS.Commands.Internal.UserRegisterBulk;
using HowCommon.MassTransitContract;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

public class UserRegisterBulkConsumer : IConsumer<UserRegiserBulkMessage>
{
    private readonly ILogger<UserRegisterBulkConsumer> _logger;
    private readonly ISender _sender;

    public UserRegisterBulkConsumer(ILogger<UserRegisterBulkConsumer> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<UserRegiserBulkMessage> context)
    {
        _logger.LogInformation("Content Received Bulk User ID");

        if (context.Message.UserIds.Length == 0)
        {
            _logger.LogError("Received User Register Bulk Message without User IDs");
        }

        var result = await _sender.Send(new InternalUserRegisterBulkCommand
        {
            UserIds = context.Message.UserIds
        });

        if (result.Succeeded)
        {
            _logger.LogInformation($"Users was registered successfully.");
        }
        else
        {
            _logger.LogError(result.GetErrorMessages());
        }
    }
}