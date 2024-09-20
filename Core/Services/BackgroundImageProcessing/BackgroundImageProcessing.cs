namespace How.Core.Services.BackgroundImageProcessing;

using CQRS.Commands.Record.CreateRecordImages;
using CQRS.Commands.Storage.CreateImageMultiply;
using CQRS.Commands.Storage.DeleteImageMultiply;
using CQRS.Queries.Record.GetMaxImagePosition;
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

    public async Task RecordImageProcessing(int userId, int recordId, List<(string name, byte[] content)> files)
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
                RollBackImageProcessing(imageIds);
                
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
                RollBackImageProcessing(imageIds);
                
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
            RollBackImageProcessing(imageIds);
            
            _logger.LogError(e.Message);
            await _fileProcessing.NotifyUser(userId, "Images processing failed!");
        }
    }

    private void RollBackImageProcessing(int[] imageIds)
    {
        if (imageIds.Any())
        {
            _sender.Send(new DeleteImageMultiplyCommand
            {
                ImageIds = imageIds
            });
        }
    }
}