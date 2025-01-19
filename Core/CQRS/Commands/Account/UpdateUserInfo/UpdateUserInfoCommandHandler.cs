namespace How.Core.CQRS.Commands.Account.UpdateUserInfo;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Identity;
using Microsoft.Extensions.Logging;

public class UpdateUserInfoCommandHandler : ICommandHandler<UpdateUserInfoCommand, Result<int>>
{
    private readonly ILogger<UpdateUserInfoCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public UpdateUserInfoCommandHandler(ILogger<UpdateUserInfoCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
UPDATE {nameof(BaseDbContext.Users).ToSnake()}
SET 
    {nameof(HowUser.FirstName).ToSnake()} = @first_name,
    {nameof(HowUser.LastName).ToSnake()} = @last_name
WHERE {nameof(HowUser.UserId).ToSnake()} = @userId
RETURNING *;
";
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command, 
                new
                {
                    first_name = request.FirstName,
                    last_name = request.LastName,
                    userId = request.CurrentUserId
                });

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Account, $"Error while executing {nameof(UpdateUserInfoCommand)}"));
        }
    }
}