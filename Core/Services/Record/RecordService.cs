namespace How.Core.Services.Record;

using Common.Extensions;
using Common.ResultType;
using CQRS.Commands.Record.InsertRecord;
using CQRS.Commands.Record.UpdateRecord;
using CQRS.Queries.General.CheckExist;
using CurrentUser;
using Database;
using DTO.Record;
using MediatR;
using Microsoft.Extensions.Logging;

public class RecordService : IRecordService
{
    private readonly ILogger<RecordService> _logger;
    private readonly ISender _sender;
    private readonly ICurrentUserService _userService;

    public RecordService(ILogger<RecordService> logger, ISender sender, ICurrentUserService userService)
    {
        _logger = logger;
        _sender = sender;
        _userService = userService;
    }

    public async Task<Result<int>> CreateRecord(int eventId, CreateRecordRequestDTO request)
    {
        try
        {
            var eventExist = await _sender.Send(new CheckExistQuery
            {
                Id = eventId,
                Table = nameof(BaseDbContext.Events).ToSnake()
            });

            if (eventExist.Failed)
            {
                return Result.Failure<int>(eventExist.Error);
            }

            if (!eventExist.Data)
            {
                return Result.Failure<int>(
                    new Error(ErrorType.Record, $"Event not found!"));
            }
            
            var command = new InsertRecordCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                Description = request.Description.Trim()
            };

            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure<int>(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure<int>(
                    new Error(ErrorType.Record, $"Record not created!"));
            }
            
            return Result.Success(result.Data);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Record, $"Error at {nameof(CreateRecord)}"));
        }
    }

    public async Task<Result> UpdateRecord(int eventId, int recordId, UpdateRecordRequestDTO request)
    {
        try
        {
            var command = new UpdateRecordCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                RecordId = recordId,
                Description = request.Description
            };
            
            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure(
                    new Error(ErrorType.Record, $"Record not updated!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Record, $"Error at {nameof(UpdateRecord)}"));;
        }
    }
}