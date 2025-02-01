namespace How.Core.CQRS.Commands.Internal.UserDelete;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Identity;
using HowCommon.Extensions;
using Microsoft.Extensions.Logging;

public class InternalUserDeleteCommandHandler : ICommandHandler<InternalUserDeleteCommand, Result<int>>
{
    private readonly ILogger<InternalUserDeleteCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public InternalUserDeleteCommandHandler(ILogger<InternalUserDeleteCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(InternalUserDeleteCommand request, CancellationToken cancellationToken)
    {
        try
        {
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

            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command,
                new
                {
                    UserId = request.UserId,
                    FirstName = request.Salt.ToUpper(),
                    LastName = request.Salt.ToUpper()
                });

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Internal, $"Error while executing {nameof(InternalUserDeleteCommand)}"));
        }
    }
}