namespace How.Core.Services.Record;

using Common.Extensions;
using Common.ResultType;
using CQRS.Commands.Record.CreateRecordImages;
using CQRS.Commands.Record.DeleteRecordImages;
using CQRS.Commands.Record.InsertRecord;
using CQRS.Commands.Record.UpdateRecord;
using CQRS.Commands.Record.UpdateRecordImagePosition;
using CQRS.Commands.Storage.CreateImageMultiply;
using CQRS.Commands.Storage.DeleteImageMultiply;
using CQRS.Queries.General.CheckExistForUser;
using CQRS.Queries.Record.GetImageIds;
using CQRS.Queries.Record.GetMaxImagePosition;
using CurrentUser;
using Database;
using DTO.Models;
using DTO.Record;
using DTO.RecordImage;
using MediatR;
using Microsoft.Extensions.Logging;
using Models.ServicesModel;
using Storage.ImageStorage;

public class RecordService : IRecordService
{
    private readonly ILogger<RecordService> _logger;
    private readonly ISender _sender;
    private readonly ICurrentUserService _userService;
    private readonly IImageStorageService _imageStorage;

    public RecordService(
        ILogger<RecordService> logger,
        ISender sender,
        ICurrentUserService userService,
        IImageStorageService imageStorage)
    {
        _logger = logger;
        _sender = sender;
        _userService = userService;
        _imageStorage = imageStorage;
    }

    public async Task<Result<int>> CreateRecord(int eventId, CreateRecordRequestDTO request)
    {
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

    public async Task<Result> UpdateRecord(int recordId, UpdateRecordRequestDTO request)
    {
        try
        {
            var command = new UpdateRecordCommand
            {
                CurrentUserId = _userService.UserId,
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

    public async Task<Result<CreateRecordImagesResponseDTO>> CreateRecordImages(
        int recordId,
        CreateRecordImagesRequestDTO request)
    {
        var imageIds = new int[request.Files.Count];
        try
        {
            var recordExist = await _sender.Send(new CheckExistForUserQuery
            {
                Id = recordId,
                CurrentUserId = _userService.UserId,
                Table = nameof(BaseDbContext.Records).ToSnake()
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
            
            var imagesInternal = new List<ImageInternalModel>();
            
            foreach (var item in request.Files)
            {
                var image = await _imageStorage.CreateImageInternal(item);

                if (image.Failed)
                {
                    return Result.Failure<CreateRecordImagesResponseDTO>(image.Error);
                }
                
                imagesInternal.Add(image.Data);
            }
            
            var createImages = await _sender.Send(new CreateImageMultiplyCommand
            {
                Images = imagesInternal
            });

            if (createImages.Failed)
            {
                return Result.Failure<CreateRecordImagesResponseDTO>(createImages.Error);
            }

            if (!createImages.Data.Any())
            {
                return Result.Failure<CreateRecordImagesResponseDTO>(
                    new Error(ErrorType.Record, $"Images not created!"));
            }

            imageIds = createImages.Data;

            var maxPosition = await _sender.Send(new GetMaxImagePositionQuery
            {
                RecordId = recordId
            });
            
            if (maxPosition.Failed)
            {
                return Result.Failure<CreateRecordImagesResponseDTO>(maxPosition.Error);
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
                return Result.Failure<CreateRecordImagesResponseDTO>(createImages.Error);
            }

            if (!commandResult.Data.Any())
            {
                return Result.Failure<CreateRecordImagesResponseDTO>(
                    new Error(ErrorType.Record, $"Record Images not created!"));
            }
            
            var result = new CreateRecordImagesResponseDTO
            {
                ImagePaths = imagesInternal.Select(i => new UploadImageResponseModelDTO
                {
                    MainHash = i.Main.Hash,
                    ThumbnailHash = i.Thumbnail.Hash
                }).ToList()
            };

            return new Result<CreateRecordImagesResponseDTO>(result);
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

    public async Task<Result> UpdateRecordImages(int recordId, UpdateRecordImagesRequestDTO request)
    {
        try
        {
            var recordExist = await _sender.Send(new CheckExistForUserQuery
            {
                Id = recordId,
                CurrentUserId = _userService.UserId,
                Table = nameof(BaseDbContext.Records).ToSnake()
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
}