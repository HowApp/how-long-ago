namespace How.Core.CQRS.Commands.SharedUser.CreateSharedUser;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.SharedUser;
using Microsoft.Extensions.Logging;

public class CreateSharedUserCommandHandler : ICommandHandler<CreateSharedUserCommand, Result<int>>
{
    private readonly ILogger<CreateSharedUserCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public CreateSharedUserCommandHandler(ILogger<CreateSharedUserCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(CreateSharedUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
INSERT INTO {nameof(BaseDbContext.SharedUsers).ToSnake()} (
    {nameof(SharedUser.UserOwnerId).ToSnake()},
    {nameof(SharedUser.UserSharedId).ToSnake()}
)
VALUES (@ownerId, @sharedId)
ON CONFLICT({nameof(SharedUser.UserOwnerId).ToSnake()}, {nameof(SharedUser.UserSharedId).ToSnake()})
DO NOTHING; 

SELECT *
FROM shared_users su
WHERE su.user_owner_id = @ownerId AND su.user_shared_id = @sharedId;
";
            await using var connection = _dapper.InitConnection();
            var result = await connection.QuerySingleAsync<int>(
                command,
                new
                {
                    ownerId = request.CurrentUserId,
                    sharedId = request.SharedUserId
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.SharedUser, $"Error while executing {nameof(CreateSharedUserCommand)}"));
        }
    }
}