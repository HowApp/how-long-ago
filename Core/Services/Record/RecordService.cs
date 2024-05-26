namespace How.Core.Services.Record;

using Common.Extensions;
using Common.ResultType;
using CQRS.Commands.Record.InsertRecord;
using CQRS.Commands.Record.UpdateRecord;
using CQRS.Commands.Storage.CreateImageMultiply;
using CQRS.Commands.Storage.DeleteImageMultiply;
using CQRS.Queries.General.CheckExistForUser;
using CQRS.Queries.Record.CheckRecordExist;
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
                new Error(ErrorType.Record, $"Error at {nameof(UpdateRecord)}"));
        }
    }

    public async Task<Result<CreateRecordImagesResponseDTO>> CreateRecordImage(
        int eventId,
        int recordId,
        CreateRecordImagesRequestDTO request)
    {
        var imageIds = new int[request.Files.Count];
        try
        {
            var recordExist = await _sender.Send(new CheckRecordExistQuery
            {
                Id = recordId,
                CurrentUserId = _userService.UserId,
                EventId = eventId
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
                new Error(ErrorType.Record, $"Error at {nameof(CreateRecordImage)}"));
        }
    }
}