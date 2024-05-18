namespace How.Core.Services.Event;

using Common.ResultType;
using CQRS.Commands.Event.CreateEvent;
using CQRS.Queries.Event.GetEventsPagination;
using CurrentUser;
using DTO.Event;
using DTO.Models;
using MediatR;
using Microsoft.Extensions.Logging;

public class EventService : IEventService
{
    private readonly ILogger<EventService> _logger;
    private readonly ISender _sender;
    private readonly ICurrentUserService _userService;

    public EventService(ILogger<EventService> logger, ISender sender, ICurrentUserService userService)
    {
        _logger = logger;
        _sender = sender;
        _userService = userService;
    }

    public async Task<Result<int>> CreateEvent(CreateEventRequestDTO request)
    {
        try
        {
            var command = new CreateEventCommand
            {
                CurrentUserId = _userService.UserId,
                Name = request.Name.Trim()
            };

            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure<int>(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure<int>(
                    new Error(ErrorType.Event, $"Event not created!"));
            }
            
            return Result.Success(result.Data);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Event, $"Error at {nameof(CreateEvent)}"));
        }
    }

    public async Task<Result<GetEventsPaginationResponseDTO>> GetEventsPagination(GetEventsPaginationRequestDTO request)
    {
        try
        {
            var query = new GetEventsPaginationQuery
            {
                Offset = (request.Page - 1) * request.Size,
                Size = request.Size,
                Search = request.Search,
                Status = request.Status
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
                        Image = new ImageModelDTO
                        {
                            MainHash = eventItem.EventMainHash,
                            ThumbnailHash = eventItem.EventThumbnailHash
                        },
                        Owner = new OwnerModelDTO
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