namespace How.Core.Infrastructure.Processing.Consumer;

using Dapper;
using Database;
using Database.Entities.Identity;
using HowCommon.Extensions;
using HowCommon.MassTransitContract;
using MassTransit;
using Microsoft.Extensions.Logging;

public class UserDeletedConsumer : IConsumer<UserDeletedMessage>
{
    private readonly DapperConnection _dapper;
    private readonly ILogger<UserDeletedConsumer> _logger;

    public UserDeletedConsumer(DapperConnection dapper, ILogger<UserDeletedConsumer> logger)
    {
        _dapper = dapper;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<UserDeletedMessage> context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", context.Message.UserId);

        if (context.Message.UserId == 0)
        {
            _logger.LogError("Received User Deleted Message without User ID");
        }
        
        var salt = "Deleted_" + Guid.NewGuid();
        
        var command = $@"
UPDATE {nameof(BaseDbContext.Users).ToSnake()} 
SET
    {nameof(HowUser.IsDeleted).ToSnake()} = true,
    {nameof(HowUser.IsSuspended).ToSnake()} = true,
    {nameof(HowUser.FirstName).ToSnake()} = @FirstName,
    {nameof(HowUser.LastName).ToSnake()} = @LastName
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
                    FirstName = salt.ToUpper(),
                    LastName = salt.ToUpper()
                });

            if (result == 0)
            {
                _logger.LogInformation($"User not deleted. User ID: {context.Message.UserId}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}