namespace How.Core.Services.Record;

using Common.ResultType;
using CQRS.Commands.Record.CreateRecordImages;
using CQRS.Commands.Record.DeleteRecord;
using CQRS.Commands.Record.DeleteRecordImages;
using CQRS.Commands.Record.InsertRecord;
using CQRS.Commands.Record.UpdateRecord;
using CQRS.Commands.Record.UpdateRecordImagePosition;
using CQRS.Commands.Record.UpdateRecordLikeState;
using CQRS.Commands.Storage.CreateImageMultiply;
using CQRS.Commands.Storage.DeleteImageMultiply;
using CQRS.Queries.General.CheckExistAccess;
using CQRS.Queries.Record.GetImageIds;
using CQRS.Queries.Record.GetMaxImagePosition;
using CQRS.Queries.Record.GetRecordsPagination;
using CurrentUser;
using DTO.Models;
using DTO.Record;
using DTO.RecordImage;
using Hubs.FileProcessingHubService;
using Infrastructure.Background.BackgroundTaskQueue;
using Infrastructure.Builders;
using Infrastructure.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models.ServicesModel;
using Storage.ImageStorage;

public class RecordService : IRecordService
{
    private readonly ILogger<RecordService> _logger;
    private readonly ISender _sender;
    private readonly ICurrentUserService _userService;
    private readonly IImageStorageService _imageStorage;
    private readonly IFileProcessingHubService _fileProcessing;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public RecordService(
        ILogger<RecordService> logger,
        ISender sender,
        ICurrentUserService userService,
        IImageStorageService imageStorage,
        IFileProcessingHubService fileProcessing,
        IBackgroundTaskQueue backgroundTaskQueue)
    {
        _logger = logger;
        _sender = sender;
        _userService = userService;
        _imageStorage = imageStorage;
        _fileProcessing = fileProcessing;
        _backgroundTaskQueue = backgroundTaskQueue;
    }

    public async Task<Result<int>> CreateRecord(
        int eventId,
        CreateRecordRequestDTO request)
    {
        try
        {
            var queryBuilder = new EventAccessQueryAccessBuilder(eventId);
            queryBuilder.FilterCreatedBy(_userService.UserId, AccessFilterType.IncludeShared);

            var eventExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });

            if (eventExist.Failed)
            {
                return Result.Failure<int>(eventExist.Error);
            }

            if (!eventExist.Data)
            {
                return Result.Failure<int>(
                    new Error(ErrorType.Record, $"Event not found!"), 404);
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

    public async Task<Result<GetRecordsPaginationResponseDTO>> GetRecordsPagination(
        int eventId,
        GetRecordsPaginationRequestDTO request)
    {
        try
        {
            var queryBuilder = new EventAccessQueryAccessBuilder(eventId);
            queryBuilder.FilterCreatedBy(_userService.UserId, AccessFilterType.IncludeShared);

            var eventExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });

            if (eventExist.Failed)
            {
                return Result.Failure<GetRecordsPaginationResponseDTO>(eventExist.Error);
            }

            if (!eventExist.Data)
            {
                return Result.Failure<GetRecordsPaginationResponseDTO>(
                    new Error(ErrorType.Record, $"Event not found!"), 404);
            }

            var query = new GetRecordsPaginationQuery
            {
                CurrentUserId = _userService.UserId,
                Offset = (request.Page - 1) * request.Size,
                Size = request.Size,
                EventId = eventId
            };
            
            var queryResult = await _sender.Send(query);
            
            if (queryResult.Failed)
            {
                return Result.Failure<GetRecordsPaginationResponseDTO>(queryResult.Error);
            }

            return new Result<GetRecordsPaginationResponseDTO>(new GetRecordsPaginationResponseDTO
            {
                Count = queryResult.Data.Count,
                Records = queryResult.Data.Records
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetRecordsPaginationResponseDTO>(
                new Error(ErrorType.Record, $"Error at {nameof(GetRecordsPagination)}"));
        }
    }

    public async Task<Result> UpdateRecord(
        int eventId,
        int recordId,
        UpdateRecordRequestDTO request)
    {
        try
        {
            var queryBuilder = new RecordAccessAccessBuilder(eventId, recordId);
            queryBuilder.FilterCreatedBy(_userService.UserId, AccessFilterType.IncludeShared);
            
            var recordExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });
                
            if (recordExist.Failed)
            {
                return Result.Failure(recordExist.Error);
            }

            if (!recordExist.Data)
            {
                return Result.Failure(
                    new Error(ErrorType.Record, $"Record not found!"), 404);
            }
            
            var command = new UpdateRecordCommand
            {
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
                new Error(ErrorType.Record, $"Error at {nameof(UpdateRecord)}"));
        }
    }

    public async Task<Result<LikeState>> UpdateLikeState(
        int eventId,
        int recordId,
        LikeState likeState)
    {
        try
        {
            var queryBuilder = new RecordAccessAccessBuilder(eventId, recordId);
            queryBuilder.FilterCreatedBy(_userService.UserId, AccessFilterType.IncludeShared);
            queryBuilder.FilterByStatus(EventStatus.Active);
            
            var recordExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });
            
            if (recordExist.Failed)
            {
                return Result.Failure<LikeState>(recordExist.Error);
            }

            if (!recordExist.Data)
            {
                return Result.Failure<LikeState>(new Error(ErrorType.Record, $"Record not found!"), 404);
            }
            
            var command = new UpdateRecordLikeStateCommand
            {
                CurrentUserId = _userService.UserId,
                RecordId = recordId,
                LikeState = likeState
            };
            
            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure<LikeState>(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure<LikeState>(new Error(ErrorType.Record, "Action not performed!"));
            }
            
            return Result.Success(likeState);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<LikeState>(
                new Error(ErrorType.Record, $"Error at {nameof(UpdateLikeState)}"));
        }
    }

    public async Task<Result<CreateRecordImagesResponseDTO>> CreateRecordImages(
        int eventId,
        int recordId,
        CreateRecordImagesRequestDTO request)
    {
        var imageIds = new int[request.Files.Count];
        var userId = _userService.UserId;
        try
        {
            var queryBuilder = new RecordAccessAccessBuilder(eventId, recordId);
            queryBuilder.FilterCreatedBy(userId, AccessFilterType.IncludeShared);
            
            var recordExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });
                
            if (recordExist.Failed)
            {
                return Result.Failure<CreateRecordImagesResponseDTO>(recordExist.Error);
            }

            if (!recordExist.Data)
            {
                return Result.Failure<CreateRecordImagesResponseDTO>(
                    new Error(ErrorType.Record, $"Record not found!"), 404);
            }
            
            List<(string nameof, byte[] content)> files = new List<(string nameof, byte[] content)>(request.Files.Count);

            foreach (var t in request.Files)
            {
                using var memoryStream = new MemoryStream();
                await t.CopyToAsync(memoryStream);
                files.Add((t.FileName, memoryStream.ToArray()));
            }
            
            _backgroundTaskQueue.QueueBackgroundWorkItem(async (scope, token) =>
            {
                _logger.LogInformation("Start processing records.");
                
                // Resolve services inside the scope
                var recordService = scope.ServiceProvider.GetRequiredService<IRecordService>();

                await recordService.ImageProcessing(userId, recordId, files);
                
                _logger.LogInformation("Complete processing records.");
            });
            
            // var imagesInternal = new List<ImageInternalModel>();
            //
            // foreach (var item in request.Files)
            // {
            //     var image = await _imageStorage.CreateImageInternal(item);
            //
            //     if (image.Failed)
            //     {
            //         return Result.Failure<CreateRecordImagesResponseDTO>(image.Error);
            //     }
            //     
            //     imagesInternal.Add(image.Data);
            // }
            //
            // var createImages = await _sender.Send(new CreateImageMultiplyCommand
            // {
            //     Images = imagesInternal
            // });
            //
            // if (createImages.Failed)
            // {
            //     return Result.Failure<CreateRecordImagesResponseDTO>(createImages.Error);
            // }
            //
            // if (!createImages.Data.Any())
            // {
            //     return Result.Failure<CreateRecordImagesResponseDTO>(
            //         new Error(ErrorType.Record, $"Images not created!"));
            // }
            //
            // imageIds = createImages.Data;
            //
            // var maxPosition = await _sender.Send(new GetMaxImagePositionQuery
            // {
            //     RecordId = recordId
            // });
            //
            // if (maxPosition.Failed)
            // {
            //     return Result.Failure<CreateRecordImagesResponseDTO>(maxPosition.Error);
            // }
            //
            // var command = new CreateRecordImagesCommand
            // {
            //     RecordId = recordId,
            //     ImageIds = imageIds,
            //     MaxPosition = ++maxPosition.Data
            // };
            //
            // var commandResult = await _sender.Send(command);
            //
            // if (commandResult.Failed)
            // {
            //     return Result.Failure<CreateRecordImagesResponseDTO>(createImages.Error);
            // }
            //
            // if (!commandResult.Data.Any())
            // {
            //     return Result.Failure<CreateRecordImagesResponseDTO>(
            //         new Error(ErrorType.Record, $"Record Images not created!"));
            // }
            //
            // var result = new CreateRecordImagesResponseDTO
            // {
            //     ImagePaths = imagesInternal.Select(i => new UploadImageResponseModelDTO
            //     {
            //         MainHash = i.Main.Hash,
            //         ThumbnailHash = i.Thumbnail.Hash
            //     }).ToList()
            // };

            // return new Result<CreateRecordImagesResponseDTO>(result);

            return Result.Success<CreateRecordImagesResponseDTO>(new CreateRecordImagesResponseDTO());
        }
        catch (Exception e)
        {
            if (imageIds.Any())
            {
                await _sender.Send(new DeleteImageMultiplyCommand
                {
                    ImageIds = imageIds
                });
            }
            
            _logger.LogError(e.Message);
            return Result.Failure<CreateRecordImagesResponseDTO>(
                new Error(ErrorType.Record, $"Error at {nameof(CreateRecordImages)}"));
        }
    }
    
    public async Task ImageProcessing(
        int userId,
        int recordId,
        List<(string name, byte[] content)> files)
    {
        var imageIds = new int[files.Count];
        try
        {
            var imagesInternal = new List<ImageInternalModel>();
            
            foreach (var item in files)
            {
                var image = await _imageStorage.CreateImageInternal(item.content, item.name);

                if (image.Failed)
                {
                    _logger.LogError(image.GetErrorMessages());
                    await _fileProcessing.NotifyUser(userId, image.GetErrorMessages());
                    return;
                }
                
                imagesInternal.Add(image.Data);
            }
            
            var createImages = await _sender.Send(new CreateImageMultiplyCommand
            {
                Images = imagesInternal
            });

            if (createImages.Failed)
            {
                _logger.LogError(createImages.GetErrorMessages());
                await _fileProcessing.NotifyUser(userId, createImages.GetErrorMessages());
                return;
            }

            if (!createImages.Data.Any())
            {
                _logger.LogError("Images not created!");
                await _fileProcessing.NotifyUser(userId, "Images not created!");
                return;
            }

            imageIds = createImages.Data;

            var maxPosition = await _sender.Send(new GetMaxImagePositionQuery
            {
                RecordId = recordId
            });
            
            if (maxPosition.Failed)
            {
                _logger.LogError(maxPosition.GetErrorMessages());
                await _fileProcessing.NotifyUser(userId, maxPosition.GetErrorMessages());
                return;
            }
            
            var command = new CreateRecordImagesCommand
            {
                RecordId = recordId,
                ImageIds = imageIds,
                MaxPosition = ++maxPosition.Data
            };

            var commandResult = await _sender.Send(command);

            if (commandResult.Failed)
            {
                _logger.LogError(createImages.GetErrorMessages());
                return;
            }

            if (!commandResult.Data.Any())
            {
                _logger.LogError("Record Images not created!");
                await _fileProcessing.NotifyUser(userId, "Record Images not created!");
                return;
            }
            
            var result = new CreateRecordImagesResponseDTO
            {
                ImagePaths = imagesInternal.Select(i => new UploadImageResponseModelDTO
                {
                    MainHash = i.Main.Hash,
                    ThumbnailHash = i.Thumbnail.Hash
                }).ToList()
            };
            
            await _fileProcessing.NotifyUser(userId, "Images processing finish");
        }
        catch (Exception e)
        {
            if (imageIds.Any())
            {
                await _sender.Send(new DeleteImageMultiplyCommand
                {
                    ImageIds = imageIds
                });
            }
            
            _logger.LogError(e.Message);
            await _fileProcessing.NotifyUser(userId, "Images processing failed!");
        }
    }

    public async Task<Result> UpdateRecordImages(
        int eventId,
        int recordId,
        UpdateRecordImagesRequestDTO request)
    {
        try
        {
            var queryBuilder = new RecordAccessAccessBuilder(eventId, recordId);
            queryBuilder.FilterCreatedBy(_userService.UserId, AccessFilterType.IncludeShared);
            
            var recordExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });
                
            if (recordExist.Failed)
            {
                return Result.Failure(recordExist.Error);
            }

            if (!recordExist.Data)
            {
                return Result.Failure(
                    new Error(ErrorType.Record, $"Record not found!"), 404);
            }

            var imageExistIds = await _sender.Send(new GetImageIdsQuery
            {
                RecordId = recordId
            });
            
            if (imageExistIds.Failed)
            {
                return Result.Failure(imageExistIds.Error);
            }

            if (request.ImageIds.Any(i => !imageExistIds.Data.Contains(i)))
            {
                return Result.Failure(
                    new Error(ErrorType.Record, $"Record Image not found!"), 404);
            }

            var recordImageToDelete = imageExistIds.Data.Where(i => !request.ImageIds.Contains(i)).ToArray();

            if (request.ImageIds.Any())
            {
                var updateResult = await _sender.Send(new UpdateRecordImagePositionCommand
                {
                    ImageIds = request.ImageIds
                });

                if (updateResult.Failed)
                {
                    return Result.Failure(updateResult.Error);
                }

                if (updateResult.Data != request.ImageIds.Length )
                {
                    return Result.Failure(
                        new Error(ErrorType.Record, $"Image position not updated!"));
                }
            }

            if (recordImageToDelete.Any())
            {
                var deleteRecordImageResult = await _sender.Send(new DeleteRecordImagesCommand
                {
                    ImageIds = recordImageToDelete
                });
                
                if (deleteRecordImageResult.Failed)
                {
                    return Result.Failure(deleteRecordImageResult.Error);
                }

                var deleteStorageItemsResult = await _sender.Send(new DeleteImageMultiplyCommand
                {
                    ImageIds = deleteRecordImageResult.Data
                });
                
                if (deleteStorageItemsResult.Failed)
                {
                    return Result.Failure(deleteStorageItemsResult.Error);
                }
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Record, $"Error at {nameof(UpdateRecordImages)}"));
        }
    }

    public async Task<Result> DeleteRecord(
        int eventId,
        int recordId)
    {
        try
        {
            var queryBuilder = new RecordAccessAccessBuilder(eventId, recordId);
            queryBuilder.FilterCreatedBy(_userService.UserId, AccessFilterType.IncludeShared);
            
            var recordExist = await _sender.Send(new CheckExistAccessQuery
            {
                QueryAccessBuilder = queryBuilder
            });
                
            if (recordExist.Failed)
            {
                return Result.Failure(recordExist.Error);
            }

            if (!recordExist.Data)
            {
                return Result.Failure(
                    new Error(ErrorType.Record, $"Record not found!"), 404);
            }
            
            var imageExistIds = await _sender.Send(new GetImageIdsQuery
            {
                RecordId = recordId
            });
            
            if (imageExistIds.Failed)
            {
                return Result.Failure(imageExistIds.Error);
            }

            if (imageExistIds.Data.Length > 0)
            {
                var deleteRecordImageResult = await _sender.Send(new DeleteRecordImagesCommand
                {
                    ImageIds = imageExistIds.Data
                });
                
                if (deleteRecordImageResult.Failed)
                {
                    return Result.Failure(deleteRecordImageResult.Error);
                }

                var deleteStorageItemsResult = await _sender.Send(new DeleteImageMultiplyCommand
                {
                    ImageIds = deleteRecordImageResult.Data
                });
                
                if (deleteStorageItemsResult.Failed)
                {
                    return Result.Failure(deleteStorageItemsResult.Error);
                }
            }

            var deleteRecord = await _sender.Send(new DeleteRecordCommand
            {
                RecordIds = new []{recordId}
            });
            
            if (deleteRecord.Failed)
            {
                return Result.Failure(deleteRecord.Error);
            }
            
            if (deleteRecord.Data < 1)
            {
                return Result.Failure(
                    new Error(ErrorType.Event, $"Record not deleted!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Record, $"Error at {nameof(DeleteRecord)}"));
        }
    }
}