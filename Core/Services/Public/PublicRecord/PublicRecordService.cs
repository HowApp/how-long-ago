namespace How.Core.Services.Public.PublicRecord;

using Common.ResultType;
using CQRS.Queries.General.CheckExistAccess;
using CQRS.Queries.Public.Record.GetRecordsPaginationPublic;
using DTO.Public.Record;
using DTO.Record;
using Infrastructure.Builders;
using MediatR;
using Microsoft.Extensions.Logging;

public class PublicRecordService : IPublicRecordService
{
    private readonly ILogger<PublicRecordService> _logger;
    private readonly ISender _sender;

    public PublicRecordService(
        ILogger<PublicRecordService> logger,
        ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task<Result<GetRecordsPaginationPublicResponseDTO>> GetRecordsPagination(
        int eventId,
        GetRecordsPaginationRequestDTO request)
    {
        try
        {
            var queryBuilder = new EventAccessQueryAccessBuilder(eventId);

            var eventExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });

            if (eventExist.Failed)
            {
                return Result.Failure<GetRecordsPaginationPublicResponseDTO>(eventExist.Error);
            }

            if (!eventExist.Data)
            {
                return Result.Failure<GetRecordsPaginationPublicResponseDTO>(
                    new Error(ErrorType.Record, $"Event not found!"), 404);
            }
            
            var query = new GetRecordsPaginationPublicQuery
            {
                Offset = (request.Page - 1) * request.Size,
                Size = request.Size,
                EventId = eventId
            };
            
            var queryResult = await _sender.Send(query);
            
            if (queryResult.Failed)
            {
                return Result.Failure<GetRecordsPaginationPublicResponseDTO>(queryResult.Error);
            }

            return new Result<GetRecordsPaginationPublicResponseDTO>(new GetRecordsPaginationPublicResponseDTO
            {
                Count = queryResult.Data.Count,
                Records = queryResult.Data.Records
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetRecordsPaginationPublicResponseDTO>(
                new Error(ErrorType.Record, $"Error at {nameof(GetRecordsPagination)}"));
        }
    }
}