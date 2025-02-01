namespace How.Core.CQRS.Commands.Internal.UserRegister;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Identity;
using HowCommon.Extensions;
using Microsoft.Extensions.Logging;

public class UserRegisterCommandHandler : ICommandHandler<UserRegisterCommand, Result<int>>
{
    private readonly ILogger<UserRegisterCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public UserRegisterCommandHandler(ILogger<UserRegisterCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
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
            
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command,
                new
                {
                    UserId = request.UserId
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Internal, $"Error while executing {nameof(UserRegisterCommand)}"));
        }
    }
}