namespace How.Core.CQRS.Commands.SharedUser.DeleteSharedUser;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.SharedUser;
using Microsoft.Extensions.Logging;

public class DeleteSharedUserCommandHandler : ICommandHandler<DeleteSharedUserCommand, Result<int>>
{
    private readonly ILogger<DeleteSharedUserCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public DeleteSharedUserCommandHandler(ILogger<DeleteSharedUserCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(DeleteSharedUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var removeSharedUserSql = $@"
DELETE FROM {nameof(BaseDbContext.SharedUsers).ToSnake()}
WHERE
{nameof(SharedUser.UserOwnerId).ToSnake()} = @currentUserId
    AND
{nameof(SharedUser.UserSharedId).ToSnake()} = @sharedUserId
RETURNING *;
";
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                removeSharedUserSql, new
                {
                    currentUserId = request.CurrentUserId,
                    sharedUserId = request.SharedUserId
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Account, $"Error while executing {nameof(DeleteSharedUserCommand)}"));
        }
    }
}