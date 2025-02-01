namespace How.Core.CQRS.Commands.Internal.UserUpdateSuspend;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Identity;
using HowCommon.Extensions;
using Microsoft.Extensions.Logging;

public class InternalUserUpdateSuspendCommandHandler : ICommandHandler<InternalUserUpdateSuspendCommand, Result<int>>
{
    private readonly ILogger<InternalUserUpdateSuspendCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public InternalUserUpdateSuspendCommandHandler(ILogger<InternalUserUpdateSuspendCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(InternalUserUpdateSuspendCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
UPDATE {nameof(BaseDbContext.Users).ToSnake()} 
SET
    {nameof(HowUser.IsSuspended).ToSnake()} = @State
WHERE {nameof(HowUser.UserId).ToSnake()} = @UserId 
    AND {nameof(HowUser.IsDeleted).ToSnake()} = FALSE
RETURNING *;
";

            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command,
                new
                {
                    UserId = request.UserId,
                    State = request.State
                });

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Internal, $"Error while executing {nameof(InternalUserUpdateSuspendCommand)}"));
        }
    }
}