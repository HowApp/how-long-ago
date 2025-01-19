namespace How.Core.Services.BackgroundImageProcessing;

using Common.ResultType;
using CQRS.Commands.Event.UpdateEventImage;
using CQRS.Commands.Record.CreateRecordImages;
using CQRS.Commands.Storage.CreateImage;
using CQRS.Commands.Storage.CreateImageMultiply;
using CQRS.Commands.Storage.DeleteImageMultiply;
using CQRS.Commands.TemporaryStorage.DeleteTemporaryFile;
using CQRS.Queries.Record.GetMaxImagePosition;
using CQRS.Queries.TemporaryStorage.GetTemporaryFile;
using DTO.Dashboard.Event;
using DTO.Models;
using DTO.RecordImage;
using Hubs.FileProcessingHubService;
using MediatR;
using Microsoft.Extensions.Logging;
using Models.ServicesModel;
using Storage.ImageStorage;

public class BackgroundImageProcessing : IBackgroundImageProcessing
{
    private readonly ILogger<BackgroundImageProcessing> _logger;
    private readonly ISender _sender;
    private readonly IImageStorageService _imageStorage;
    private readonly IFileProcessingHubService _fileProcessing;
    
    private int[] _temporaryFilesIds = [];
    private int[] _processedImageIds = [];

    public BackgroundImageProcessing(
        ISender sender,
        IImageStorageService imageStorage,
        IFileProcessingHubService fileProcessing,
        ILogger<BackgroundImageProcessing> logger)
    {
        _sender = sender;
        _imageStorage = imageStorage;
        _fileProcessing = fileProcessing;
        _logger = logger;
    }

    public async Task RecordImageProcessing(int userId, int recordId, int[] fileIds)
    {
        _temporaryFilesIds = fileIds;
        try
        {
            var imagesInternal = new List<ImageInternalModel>();
            
            foreach (var item in fileIds)
            {
                var temporaryImage = await _sender.Send(new GetTemporaryFileQuery
                {
                    FileId = item
                });

                if (temporaryImage.Failed)
                {
                    _logger.LogError(temporaryImage.GetErrorMessages());
                }

                if (temporaryImage.Data is not null)
                {
                    var image = await _imageStorage.CreateImageInternal(
                        temporaryImage.Data.Content,
                        temporaryImage.Data.FileName);

                    if (image.Failed)
                    {
                        _logger.LogError(image.GetErrorMessages());
                        
                        await DeleteTemporaryFiles();
                        
                        await _fileProcessing.NotifyUser(userId, image.GetErrorMessages());
                        return;
                    }
                
                    imagesInternal.Add(image.Data);
                }
            }

            await DeleteTemporaryFiles();
            
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

            _processedImageIds = createImages.Data;

            var maxPosition = await _sender.Send(new GetMaxImagePositionQuery
            {
                RecordId = recordId
            });
            
            if (maxPosition.Failed)
            {
                await RollBackImageProcessing();
                
                _logger.LogError(maxPosition.GetErrorMessages());
                await _fileProcessing.NotifyUser(userId, maxPosition.GetErrorMessages());
                return;
            }
            
            var command = new CreateRecordImagesCommand
            {
                RecordId = recordId,
                ImageIds = _processedImageIds,
                MaxPosition = ++maxPosition.Data
            };

            var commandResult = await _sender.Send(command);

            if (commandResult.Failed)
            {
                await RollBackImageProcessing();
                
                _logger.LogError(commandResult.GetErrorMessages());
                return;
            }

            if (!commandResult.Data.Any())
            {
                await RollBackImageProcessing();
                
                _logger.LogError("Record Images not created!");
                await _fileProcessing.NotifyUser(userId, "Record Images not created!");
                return;
            }
            
            var resultImageData = new CreateRecordImagesResponseDTO
            {
                ImagePaths = imagesInternal.Select(i => new UploadImageResponseModelDTO
                {
                    MainHash = i.Main.Hash,
                    ThumbnailHash = i.Thumbnail.Hash
                }).ToList()
            };

            var result = Result.Success(resultImageData);
            await _fileProcessing.NotifyUser(userId, result.Serialize());
        }
        catch (Exception e)
        {
            await RollBackImageProcessing();
            await DeleteTemporaryFiles();
            
            _logger.LogError(e.Message);
            await _fileProcessing.NotifyUser(userId, "Images processing failed!");
        }
    }

    public async Task EventImageProcessing(int userId, int eventId, int fileId)
    {
        var imageIds = 0;
        _temporaryFilesIds = new int[]{fileId};
        try
        {
            var temporaryImage = await _sender.Send(new GetTemporaryFileQuery
            {
                FileId = fileId
            });

            if (temporaryImage.Failed)
            {
                _logger.LogError(temporaryImage.GetErrorMessages());
            }

            if (temporaryImage.Data is null)
            {
                await DeleteTemporaryFiles();
                await _fileProcessing.NotifyUser(userId, "Temporary Image is null!");
            }
            
            var image = await _imageStorage.CreateImageInternal(
                temporaryImage.Data.Content,
                temporaryImage.Data.FileName);

            if (image.Failed)
            {
                _logger.LogError(image.GetErrorMessages());
                        
                await DeleteTemporaryFiles();
                        
                await _fileProcessing.NotifyUser(userId, image.GetErrorMessages());
                return;
            }

            await DeleteTemporaryFiles();

            var createImages = await _sender.Send(new CreateImageCommand
            {
                Image = image.Data
            });

            if (createImages.Failed)
            {
                _logger.LogError(createImages.GetErrorMessages());
                await _fileProcessing.NotifyUser(userId, createImages.GetErrorMessages());
                return;
            }

            _processedImageIds = new int[]{createImages.Data};
            
            var updateEventImage = await _sender.Send(new UpdateEventImageCommand
            {
                CurrentUserId = userId,
                EventId = eventId,
                ImageId = createImages.Data
            });

            if (updateEventImage.Failed)
            {
                await RollBackImageProcessing();
                
                _logger.LogError(updateEventImage.GetErrorMessages());
                return;
            }

            var resultImageData = new UpdateEventImageResponseDTO
            {
                MainHash = image.Data.Main.Hash,
                ThumbnailHash = image.Data.Thumbnail.Hash
            };
            
            var result = Result.Success(resultImageData);
            await _fileProcessing.NotifyUser(userId, result.Serialize());
        }
        catch (Exception e)
        {
            await RollBackImageProcessing();
            await DeleteTemporaryFiles();
            
            _logger.LogError(e.Message);
            await _fileProcessing.NotifyUser(userId, "Images processing failed!");
        }
    }

    private async Task RollBackImageProcessing()
    {
        if (_processedImageIds.Any())
        {
            var result = await _sender.Send(new DeleteImageMultiplyCommand
            {
                ImageIds = _processedImageIds
            });

            if (result.Failed)
            {
                _logger.LogError(result.GetErrorMessages());
            }
        }
    }
    
    private async Task DeleteTemporaryFiles()
    {
        if (_temporaryFilesIds.Any())
        {
            var result = await _sender.Send(new DeleteTemporaryFileCommand
            {
                FileId = _temporaryFilesIds
            });
            
            if (result.Failed)
            {
                _logger.LogError(result.GetErrorMessages());
            }
        }
    }
}