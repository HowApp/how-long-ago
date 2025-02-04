namespace How.Core.CQRS.Commands.Internal.UserRegisterBulk;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Identity;
using HowCommon.Extensions;
using Microsoft.Extensions.Logging;

public class InternalUserRegisterBulkCommandHandler : ICommandHandler<InternalUserRegisterBulkCommand, Result<int>>
{
    private readonly ILogger<InternalUserRegisterBulkCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public InternalUserRegisterBulkCommandHandler(ILogger<InternalUserRegisterBulkCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(InternalUserRegisterBulkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var sequence = string.Join(",\n", request.UserIds.Select(x => $"({x}, 'FALSE', 'FALSE')"));

            var command = $@"
INSERT INTO {nameof(BaseDbContext.Users).ToSnake()} (
    {nameof(HowUser.UserId).ToSnake()},
    {nameof(HowUser.IsDeleted).ToSnake()},
    {nameof(HowUser.IsSuspended).ToSnake()} )
VALUES ({nameof(sequence)})
ON CONFLICT ({nameof(HowUser.UserId).ToSnake()})
DO NOTHING
RETURNING *;
";

            command = command.Replace($"({nameof(sequence)})", sequence);

            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(command);

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Internal, $"Error while executing {nameof(InternalUserRegisterBulkCommand)}"));
        }
    }
}