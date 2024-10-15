namespace How.Core.Services.Public.PublicEvent;

using Common.ResultType;
using CQRS.Queries.Public.Event.GetEventById;
using CQRS.Queries.Public.Event.GetEventsPaginationPublic;
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

    public async Task<Result<GetEventsPaginationPublicResponseDTO>> GetEventsPagination(
        GetEventsPaginationPublicRequestDTO publicRequest)
    {
        try
        {
            var query = new GetEventsPaginationPublicQuery
            {
                Offset = (publicRequest.Page - 1) * publicRequest.Size,
                Size = publicRequest.Size,
                Search = publicRequest.Search
            };

            var queryResult = await _sender.Send(query);

            if (queryResult.Failed)
            {
                return Result.Failure<GetEventsPaginationPublicResponseDTO>(queryResult.Error);
            }

            var result = new GetEventsPaginationPublicResponseDTO
            {
                Count = queryResult.Data.Count,
                Events = new List<EventItemPublicModelDTO>(queryResult.Data.Events.Count),
            };
            
            foreach (var eventItem in queryResult.Data.Events)
            {
                result.Events.Add(
                    new EventItemPublicModelDTO
                    {
                        Id = eventItem.Id,
                        Name = eventItem.Name,
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
                        CreatedAt = eventItem.CreatedAt,
                        Likes = eventItem.Likes,
                        Dislikes = eventItem.Dislikes,
                        SavedCount = eventItem.SavedCount,
                    });
            }
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetEventsPaginationPublicResponseDTO>(
                new Error(ErrorType.Event, $"Error at {nameof(GetEventsPagination)}"));
        }
    }

    public async Task<Result<GetEventPublicByIdResponseDTO>> GetEventById(int eventId)
    {
        try
        {
            var query = new GetEventPublicByIdQuery
            {
                EventId = eventId
            };

            var queryResult = await _sender.Send(query);

            if (queryResult.Failed)
            {
                return Result.Failure<GetEventPublicByIdResponseDTO>(queryResult.Error);
            }

            if (queryResult.Data is null)
            {
                return Result.Failure<GetEventPublicByIdResponseDTO>(
                    new Error(ErrorType.Event, $"Event not found!"), 404);
            }
            
            var result = new GetEventPublicByIdResponseDTO
            {
                Id = queryResult.Data.Id,
                Name = queryResult.Data.Name,
                Image = new ImageModelDTO
                {
                    MainHash = queryResult.Data.EventMainHash,
                    ThumbnailHash = queryResult.Data.EventThumbnailHash
                },
                UserInfo = new UserInfoModelShortDTO
                {
                    Id = queryResult.Data.OwnerId,
                    FirstName = queryResult.Data.OwnerFirstName,
                    LastName = queryResult.Data.OwnerLastName,
                    Image = new ImageModelDTO
                    {
                        MainHash = queryResult.Data.OwnerMainHash,
                        ThumbnailHash = queryResult.Data.OwnerThumbnailHash
                    }
                },
                CreatedAt = queryResult.Data.CreatedAt,
                Likes = queryResult.Data.Likes,
                Dislikes = queryResult.Data.Dislikes,
                SavedCount = queryResult.Data.SavedCount
            };

            return new Result<GetEventPublicByIdResponseDTO>(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetEventPublicByIdResponseDTO>(
                new Error(ErrorType.Event, $"Error at {nameof(GetEventById)}"));
        }
    }
}