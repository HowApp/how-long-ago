namespace How.Core.Infrastructure.Processing.Consumer;

using Dapper;
using Database;
using Database.Entities.Identity;
using HowCommon.Extensions;
using HowCommon.MassTransitContract;
using MassTransit;
using Microsoft.Extensions.Logging;

public class UserSuspendStateConsumer : IConsumer<UserSuspendedStateMessage>
{
    private readonly DapperConnection _dapper;
    private readonly ILogger<UserSuspendStateConsumer> _logger;

    public UserSuspendStateConsumer(DapperConnection dapper, ILogger<UserSuspendStateConsumer> logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserSuspendedStateMessage> context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", context.Message.UserId);

        if (context.Message.UserId == 0)
        {
            _logger.LogError("Received User Suspend State Message without User ID");
        }

        var command = $@"
UPDATE {nameof(BaseDbContext.Users).ToSnake()} 
SET
    {nameof(HowUser.IsSuspended).ToSnake()} = @State
WHERE {nameof(HowUser.UserId).ToSnake()} = @UserId 
    AND {nameof(HowUser.IsDeleted).ToSnake()} = FALSE
RETURNING *;
";

        try
        {
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command,
                new
                {
                    UserId = context.Message.UserId,
                    State = context.Message.IsSuspended
                });

            if (result == 0)
            {
                _logger.LogInformation($"User not suspended. User ID: {context.Message.UserId}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}