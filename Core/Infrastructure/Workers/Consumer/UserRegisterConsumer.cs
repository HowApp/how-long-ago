namespace How.Core.Infrastructure.Workers.Consumer;

using HowCommon.Extensions;
using Dapper;
using Database;
using Database.Entities.Identity;
using HowCommon.MassTransitContract;
using MassTransit;
using Microsoft.Extensions.Logging;

public class UserRegisterConsumer : IConsumer<UserRegisterMessage>
{
    private readonly DapperConnection _dapper;
    private readonly ILogger<UserRegisterConsumer> _logger;

    public UserRegisterConsumer(ILogger<UserRegisterConsumer> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task Consume(ConsumeContext<UserRegisterMessage> context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", context.Message.UserId);

        if (context.Message.UserId == 0)
        {
            _logger.LogError("Received User Register Message without User ID");
        }
        
        var command = $@"
INSERT INTO {nameof(BaseDbContext.Users).ToSnake()} (
    {nameof(HowUser.UserId).ToSnake()},
    {nameof(HowUser.IsDeleted).ToSnake()},
    {nameof(HowUser.IsSuspended).ToSnake()} )
VALUES (
        @UserId,
        false,
        false)
ON CONFLICT ({nameof(HowUser.UserId).ToSnake()})
DO NOTHING
RETURNING *;
";

        try
        {
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command,
                new
                {
                    UserId = context.Message.UserId
                });

            if (result == 0)
            {
                _logger.LogInformation($"User not registered. User ID: {context.Message.UserId}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}