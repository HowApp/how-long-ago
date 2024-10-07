namespace How.Core.Services.Event;

using BackgroundImageProcessing;
using Common.ResultType;
using CQRS.Commands.Event.AddEventToSaved;
using CQRS.Commands.Event.CreateEvent;
using CQRS.Commands.Event.DeleteEvent;
using CQRS.Commands.Event.DeleteEventFromSaved;
using CQRS.Commands.Event.UpdateEvent;
using CQRS.Commands.Event.UpdateEventAccess;
using CQRS.Commands.Event.UpdateEventLikeState;
using CQRS.Commands.Event.UpdateEventStatus;
using CQRS.Commands.TemporaryStorage.InsertTemporaryFile;
using CQRS.Queries.Event.GetEventById;
using CQRS.Queries.General.CheckExistAccess;
using CQRS.Queries.Event.GetEventsPagination;
using CurrentUser;
using DTO.Dashboard.Event;
using DTO.Models;
using Infrastructure.Background.BackgroundTaskQueue;
using Infrastructure.Builders;
using Infrastructure.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models.ServicesModel;

public class EventService : IEventService
{
    private readonly ILogger<EventService> _logger;
    private readonly ISender _sender;
    private readonly ICurrentUserService _userService;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public EventService(
        ILogger<EventService> logger,
        ISender sender,
        ICurrentUserService userService,
        IBackgroundTaskQueue backgroundTaskQueue)
    {
        _logger = logger;
        _sender = sender;
        _userService = userService;
        _backgroundTaskQueue = backgroundTaskQueue;
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

    public async Task<Result> UpdateActivateEventStatus(int eventId, bool setActive)
    {
        try
        {
            var command = new UpdateEventStatusCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                Status = setActive ? EventStatus.Active : EventStatus.Inactive
            };
            
            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data < 1)
            {
                var message = setActive ? "Event was not activated!" : "Event was not deactivated!";
                return Result.Failure(
                    new Error(ErrorType.Event, message));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Event, $"Error at {nameof(UpdateActivateEventStatus)}"));
        }
    }

    public async Task<Result> UpdateEventAccess(int eventId, bool setPublic)
    {
        try
        {
            var command = new UpdateEventAccessCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                Access = setPublic ? EventAccessType.Public : EventAccessType.Private
            };
            
            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data < 1)
            {
                var message = setPublic ? "Event was not made public!" : "Event was not made private!";
                return Result.Failure(
                    new Error(ErrorType.Event, message));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Event, $"Error at {nameof(UpdateEventAccess)}"));
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

    public async Task<Result> UpdateEventImage(int eventId, UpdateEventImageRequestDTO request)
    {
        var userId = _userService.UserId;
        try
        {
            var queryBuilder = new EventAccessQueryAccessBuilder(eventId);
            queryBuilder.FilterCreatedBy(userId, InternalAccessFilter.IncludeShared);

            var eventExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });

            if (eventExist.Failed)
            {
                return Result.Failure(eventExist.Error);
            }

            if (!eventExist.Data)
            {
                return Result.Failure(
                    new Error(ErrorType.Record, $"Event not found!"), 404);
            }
            
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);

            var temporaryImageId = await _sender.Send(new InsertTemporaryFileCommand
            {
                File = new TemporaryFileModel
                {
                    FileName = request.File.FileName,
                    Content = memoryStream.ToArray()
                }
            });

            if (temporaryImageId.Failed)
            {
                return Result.Failure(temporaryImageId.Error);
            }

            _backgroundTaskQueue.QueueBackgroundWorkItem(async (scope, token) =>
            {
                _logger.LogInformation("Start processing records.");

                // Resolve services inside the scope
                var recordService = scope.ServiceProvider.GetRequiredService<IBackgroundImageProcessing>();

                await recordService.EventImageProcessing(userId, eventId, temporaryImageId.Data);
                
                _logger.LogInformation("Complete processing records.");
            });
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<UpdateEventImageResponseDTO>(
                new Error(ErrorType.Event, $"Error at {nameof(UpdateEventImage)}"));
        }
    }

    public async Task<Result<LikeState>> UpdateLikeState(int eventId, LikeState likeState)
    {
        try
        {
            var queryBuilder = new EventAccessQueryAccessBuilder(eventId);
            queryBuilder.FilterByStatus(EventStatus.Active);
            queryBuilder.FilterByAccessType(EventAccessType.Public);

            var eventExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });

            if (eventExist.Failed)
            {
                return Result.Failure<LikeState>(eventExist.Error);
            }

            if (!eventExist.Data)
            {
                return Result.Failure<LikeState>(new Error(ErrorType.Event, $"Event not found!"), 404);
            }
            
            var command = new UpdateEventLikeStateCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                LikeState = likeState
            };
            
            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure<LikeState>(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure<LikeState>(new Error(ErrorType.Event, "Action not performed!"));
            }
            
            return Result.Success(likeState);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<LikeState>(
                new Error(ErrorType.Event, $"Error at {nameof(UpdateLikeState)}"));
        }
    }

    public async Task<Result<GetEventsPaginationResponseDTO>> GetEventsPagination(
        GetEventsPaginationRequestDTO request,
        InternalAccessFilter internalAccessFilter)
    {
        try
        {
            var query = new GetEventsPaginationQuery
            {
                CurrentUserId = _userService.UserId,
                Offset = (request.Page - 1) * request.Size,
                Size = request.Size,
                Search = request.Search,
                Status = request.Status,
                Access = request.Access,
                InternalAccessFilter = internalAccessFilter
            };

            var queryResult = await _sender.Send(query);

            if (queryResult.Failed)
            {
                return Result.Failure<GetEventsPaginationResponseDTO>(queryResult.Error);
            }

            var result = new GetEventsPaginationResponseDTO
            {
                Count = queryResult.Data.Count,
                Events = new List<EventItemPrivateModelDTO>(queryResult.Data.Events.Count),
            };
            
            foreach (var eventItem in queryResult.Data.Events)
            {
                result.Events.Add(
                    new EventItemPrivateModelDTO
                    {
                        Id = eventItem.Id,
                        Name = eventItem.Name,
                        Status = eventItem.Status,
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
                        CreatedAt = eventItem.CreatedAt,
                        Likes = eventItem.Likes,
                        Dislikes = eventItem.Dislikes,
                        OwnLikeState = eventItem.OwnLikeState,
                        SavedCount = eventItem.SavedCount,
                        IsSavedByUser = eventItem.IsSavedByUser,
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

    public async Task<Result<GetEventByIdResponseDTO>> GetEventById(
        int eventId,
        InternalAccessFilter internalAccessFilter = InternalAccessFilter.IncludeCreatedBy)
    {
        try
        {
            var query = new GetEventByIdQuery
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId,
                InternalAccessFilter = internalAccessFilter
            };

            var queryResult = await _sender.Send(query);

            if (queryResult.Failed)
            {
                return Result.Failure<GetEventByIdResponseDTO>(queryResult.Error);
            }

            var result = new GetEventByIdResponseDTO
                {
                    Id = queryResult.Data.Id,
                    Name = queryResult.Data.Name,
                    Status = queryResult.Data.Status,
                    Access = queryResult.Data.Access,
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
                    OwnLikeState = queryResult.Data.OwnLikeState,
                    SavedCount = queryResult.Data.SavedCount,
                    IsSavedByUser = queryResult.Data.IsSavedByUser,
                };

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetEventByIdResponseDTO>(
                new Error(ErrorType.Event, $"Error at {nameof(GetEventById)}"));
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

    public async Task<Result> AddEventToSaved(int eventId)
    {
        try
        {
            var queryBuilder = new EventAccessQueryAccessBuilder(eventId);
            queryBuilder.FilterByAccessType(EventAccessType.Public);
            queryBuilder.FilterByStatus(EventStatus.Active);
            
            var eventAccess = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });

            if (eventAccess.Failed)
            {
                return Result.Failure<UpdateEventImageResponseDTO>(eventAccess.Error);
            }

            if (!eventAccess.Data)
            {
                return Result.Failure<UpdateEventImageResponseDTO>(
                    new Error(ErrorType.Event, $"Event not found!"), 404);
            }

            var command = new AddEventToSavedCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId
            };

            var result = await _sender.Send(command);
            
            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure(
                    new Error(ErrorType.Event, $"Event not added to Saved!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Event, $"Error at {nameof(AddEventToSaved)}"));
        }
    }

    public async Task<Result> DeleteEventFromSaved(int eventId)
    {
        try
        {
            var result = await _sender.Send(new DeleteEventFromSavedCommand
            {
                CurrentUserId = _userService.UserId,
                EventId = eventId
            });
            
            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure(
                    new Error(ErrorType.Event, $"Event not deleted from Saved!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Event, $"Error at {nameof(DeleteEventFromSaved)}"));
        }
    }
}