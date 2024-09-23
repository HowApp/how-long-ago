namespace How.Core.Services.BackgroundImageProcessing;

using Common.ResultType;
using CQRS.Commands.Record.CreateRecordImages;
using CQRS.Commands.Storage.CreateImageMultiply;
using CQRS.Commands.Storage.DeleteImageMultiply;
using CQRS.Commands.TemporaryStorage.DeleteTemporaryFile;
using CQRS.Queries.Record.GetMaxImagePosition;
using CQRS.Queries.TemporaryStorage.GetTemporaryFile;
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
        var imageIds = new int[fileIds.Length];
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
                    var image = await _imageStorage.CreateImageInternal(temporaryImage.Data.Content, temporaryImage.Data.FileName);

                    if (image.Failed)
                    {
                        _logger.LogError(image.GetErrorMessages());
                        
                        await DeleteTemporaryFiles(fileIds);
                        
                        await _fileProcessing.NotifyUser(userId, image.GetErrorMessages());
                        return;
                    }
                
                    imagesInternal.Add(image.Data);
                }
            }

            await DeleteTemporaryFiles(fileIds);
            
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
                await RollBackImageProcessing(imageIds);
                
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
                await RollBackImageProcessing(imageIds);
                
                _logger.LogError(createImages.GetErrorMessages());
                return;
            }

            if (!commandResult.Data.Any())
            {
                RollBackImageProcessing(imageIds);
                
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
            await RollBackImageProcessing(imageIds);
            await DeleteTemporaryFiles(fileIds);
            
            _logger.LogError(e.Message);
            await _fileProcessing.NotifyUser(userId, "Images processing failed!");
        }
    }

    private async Task RollBackImageProcessing(int[] imageIds)
    {
        if (imageIds.Any())
        {
            var result = await _sender.Send(new DeleteImageMultiplyCommand
            {
                ImageIds = imageIds
            });

            if (result.Failed)
            {
                _logger.LogError(result.GetErrorMessages());
            }
        }
    }
    
    private async Task DeleteTemporaryFiles(int[] fileIds)
    {
        if (fileIds.Any())
        {
            var result = await _sender.Send(new DeleteTemporaryFileCommand
            {
                FileId = fileIds
            });
            
            if (result.Failed)
            {
                _logger.LogError(result.GetErrorMessages());
            }
        }
    }
}