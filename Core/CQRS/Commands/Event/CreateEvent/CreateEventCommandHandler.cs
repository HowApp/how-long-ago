namespace How.Core.CQRS.Commands.Event.CreateEvent;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Infrastructure.Enums;
using Microsoft.Extensions.Logging;
using NodaTime;

public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, Result<int>>
{
    private readonly ILogger<CreateEventCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public CreateEventCommandHandler(ILogger<CreateEventCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
INSERT INTO events (name, status, is_deleted, owner_id, crete_by_id, created_at, changed_by, changed_at) 
VALUES (@name, @status, @is_deleted, @owner_id, @crete_by_id, @created_at, @changed_by, @changed_at)
RETURNING id;
";
            await using var connection = _dapper.InitConnection();
            var result = await connection.QuerySingleAsync<int>(
                command,
                new
                {
                    name = request.Name,
                    status = (int)EventStatus.Inactive, 
                    is_deleted = false, 
                    owner_id = request.CurrentUserId, 
                    crete_by_id = request.CurrentUserId, 
                    created_at = SystemClock.Instance.GetCurrentInstant(), 
                    changed_by = request.CurrentUserId,
                    changed_at = SystemClock.Instance.GetCurrentInstant()
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Account, $"Error while executing {nameof(CreateEventCommand)}"));
        }
    }
}