namespace How.Core.Services.Public.PublicEvent;

using Common.ResultType;
using CQRS.Queries.Public.Event.GetEventsPagination;
using DTO.Public.Event;
using DTO.Models;
using MediatR;
using Microsoft.Extensions.Logging;

public class PublicEventService : IPublicEventService
{
    private readonly ILogger<PublicEventService> _logger;
    private readonly ISender _sender;

    public PublicEventService(
        ILogger<PublicEventService> logger,
        ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task<Result<GetEventsPaginationResponseDTO>> GetEventsPagination(GetEventsPaginationRequestDTO request)
    {
        try
        {
            var query = new GetEventsPaginationQuery
            {
                Offset = (request.Page - 1) * request.Size,
                Size = request.Size,
                Search = request.Search
            };

            var queryResult = await _sender.Send(query);

            if (queryResult.Failed)
            {
                return Result.Failure<GetEventsPaginationResponseDTO>(queryResult.Error);
            }

            var result = new GetEventsPaginationResponseDTO
            {
                Count = queryResult.Data.Count,
                Events = new List<EventItemModelDTO>(queryResult.Data.Events.Count),
            };
            
            foreach (var eventItem in queryResult.Data.Events)
            {
                result.Events.Add(
                    new EventItemModelDTO
                    {
                        Id = eventItem.Id,
                        Name = eventItem.Name,
                        Access = eventItem.Access,
                        Image = new ImageModelDTO
                        {
                            MainHash = eventItem.EventMainHash,
                            ThumbnailHash = eventItem.EventThumbnailHash
                        },
                        UserInfo = new UserInfoModelShortDTO
                        {
                            Id = eventItem.OwnerId,
                            FirstName = eventItem.OwnerFirstName,
                            LastName = eventItem.OwnerLastName,
                            Image = new ImageModelDTO
                            {
                                MainHash = eventItem.OwnerMainHash,
                                ThumbnailHash = eventItem.OwnerThumbnailHash
                            }
                        },
                        CreatedAt = eventItem.CreatedAt
                    });
            }
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetEventsPaginationResponseDTO>(
                new Error(ErrorType.Event, $"Error at {nameof(GetEventsPagination)}"));
        }
    }
}