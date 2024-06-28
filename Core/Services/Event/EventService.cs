namespace How.Core.Services.Event;

using Common.Extensions;
using Common.ResultType;
using CQRS.Commands.Event.CreateEvent;
using CQRS.Commands.Event.DeleteEvent;
using CQRS.Commands.Event.UpdateEvent;
using CQRS.Commands.Event.UpdateEventImage;
using CQRS.Commands.Event.UpdateEventStatus;
using CQRS.Commands.Storage.DeleteImage;
using CQRS.Commands.Storage.CreateImage;
using CQRS.Queries.Event.GetEventsPagination;
using CQRS.Queries.General.CheckExistForUser;
using CurrentUser;
using Database;
using DTO.Event;
using DTO.Models;
using Infrastructure.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Storage.ImageStorage;

public class EventService : IEventService
{
    private readonly ILogger<EventService> _logger;
    private readonly ISender _sender;
    private readonly ICurrentUserService _userService;
    private readonly IImageStorageService _imageStorage;

    public EventService(
        ILogger<EventService> logger,
        ISender sender,
        ICurrentUserService userService,
        IImageStorageService imageStorage)
    {
        _logger = logger;
        _sender = sender;
        _userService = userService;
        _imageStorage = imageStorage;
    }

    public async Task<Result<int>> CreateEvent(CreateEventRequestDTO request)
    {
        try
        {
            var command = new InsertEventCommand
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

    public async Task<Result> ActivateEvent(int eventId)
    {
        try
        {
            var command = new UpdateEventStatusCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                Status = EventStatus.Active
            };
            
            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure(
                    new Error(ErrorType.Event, $"Event not activated!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Event, $"Error at {nameof(ActivateEvent)}"));
        }
    }
    
    public async Task<Result> DeactivateEvent(int eventId)
    {
        try
        {
            var command = new UpdateEventStatusCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                Status = EventStatus.Inactive
            };
            
            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure(
                    new Error(ErrorType.Event, $"Event not deactivated!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Event, $"Error at {nameof(DeactivateEvent)}"));
        }
    }

    public async Task<Result> UpdateEvent(int eventId, UpdateEventRequestDTO request)
    {
        try
        {
            var command = new UpdateEventCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                Name = request.Name
            };
            
            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure(
                    new Error(ErrorType.Event, $"Event not updated!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Event, $"Error at {nameof(UpdateEvent)}"));
        }
    }

    public async Task<Result<UpdateEventImageResponseDTO>> UpdateEventImage(int eventId, UpdateEventImageRequestDTO request)
    {
        var imageId = 0;
        try
        {
            var eventExist = await _sender.Send(new CheckExistForUserQuery
            {
                CurrentUserId = _userService.UserId,
                Id = eventId,
                Table = nameof(BaseDbContext.Events).ToSnake()
            });

            if (eventExist.Failed)
            {
                return Result.Failure<UpdateEventImageResponseDTO>(eventExist.Error);
            }

            if (!eventExist.Data)
            {
                return Result.Failure<UpdateEventImageResponseDTO>(
                    new Error(ErrorType.Record, $"Event not found!"), 404);
            }
            
            var image = await _imageStorage.CreateImageInternal(request.File);

            if (image.Failed)
            {
                return Result.Failure<UpdateEventImageResponseDTO>(image.Error);
            }

            var insertImage = await _sender.Send(new CreateImageCommand
            {
                Image = image.Data
            });

            if (insertImage.Failed)
            {
                return Result.Failure<UpdateEventImageResponseDTO>(insertImage.Error);
            }

            if (insertImage.Data < 1)
            {
                return Result.Failure<UpdateEventImageResponseDTO>(
                    new Error(ErrorType.Event, $"Image not created!"));
            }

            imageId = insertImage.Data;
            
            var updateEventImage = await _sender.Send(new UpdateEventImageCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                ImageId = insertImage.Data
            });

            if (updateEventImage.Failed)
            {
                await _sender.Send(new DeleteImageCommand
                {
                    ImageId = insertImage.Data
                });
                return Result.Failure<UpdateEventImageResponseDTO>(updateEventImage.Error);
            }

            var result = new UpdateEventImageResponseDTO
            {
                MainHash = image.Data.Main.Hash,
                ThumbnailHash = image.Data.Thumbnail.Hash
            };
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            if (imageId != 0)
            {
                await _sender.Send(new DeleteImageCommand
                {
                    ImageId = imageId
                }); 
            }
            
            _logger.LogError(e.Message);
            return Result.Failure<UpdateEventImageResponseDTO>(
                new Error(ErrorType.Event, $"Error at {nameof(UpdateEventImage)}"));
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
                Status = request.Status,
                Access = EventAccessType.Public
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

    public async Task<Result> DeleteEvent(int eventId)
    {
        try
        {
            var command = new DeleteEventCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
            };
            
            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure(
                    new Error(ErrorType.Event, $"Event not deleted!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Event, $"Error at {nameof(UpdateEvent)}"));
        }
    }
}