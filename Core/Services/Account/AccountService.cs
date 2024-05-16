namespace How.Core.Services.Account;

using Common.ResultType;
using CQRS.Commands.Account.UpdateUserImage;
using CQRS.Commands.Account.UpdateUserInfo;
using CQRS.Commands.Storage.DeleteImage;
using CQRS.Commands.Storage.InsertImage;
using CQRS.Queries.Account.GetUserInfo;
using CurrentUser;
using DTO.Account;
using DTO.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Storage.ImageStorage;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;
    private readonly IImageStorageService _imageStorage;

    public AccountService(
        ISender sender, 
        ICurrentUserService currentUser, 
        ILogger<AccountService> logger, 
        IImageStorageService imageStorage)
    {
        _sender = sender;
        _currentUser = currentUser;
        _logger = logger;
        _imageStorage = imageStorage;
    }

    public async Task<Result<GetUserInfoResponseDTO>> GetUserInfo()
    {
        try
        {
            var query = new GetUserInfoQuery
            {
                CurrentUserId = _currentUser.UserId
            };

            var queryResult = await _sender.Send(query);

            if (queryResult.Failed)
            {
                return Result.Failure<GetUserInfoResponseDTO>(queryResult.Error);
            }

            if (queryResult.Data is null)
            {
                return Result.Failure<GetUserInfoResponseDTO>(
                    new Error(ErrorType.Account, "User not found!"));
            }

            var result = new GetUserInfoResponseDTO
            {
                Id = queryResult.Data.Id,
                FirstName = queryResult.Data.FirstName,
                LastName = queryResult.Data.LastName,
                Image = new ImageModelDTO
                {
                    MainHash = queryResult.Data.MainHash,
                    ThumbnailHash = queryResult.Data.ThumbnailHash
                }
            };

            return new Result<GetUserInfoResponseDTO>(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetUserInfoResponseDTO>(
                new Error(ErrorType.Account, $"Error at {nameof(GetUserInfo)}"));
        }
        
    }

    public async Task<Result> UpdateUserInfo(UpdateUserInfoRequestDTO request)
    {
        try
        {
            var command = new UpdateUserInfoCommand
            {
                CurrentUserId = _currentUser.UserId,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim()
            };

            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure(result.Error);
            }

            if (result.Data == 0)
            {
                return Result.Failure(new Error(ErrorType.Account, "User info not updated!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetUserInfoResponseDTO>(
                new Error(ErrorType.Account, $"Error at {nameof(UpdateUserInfo)}"));
        }
    }

    public async Task<Result<UpdateUserImageResponseDTO>> UpdateUserImage(UpdateUserImageRequestDTO request)
    {
        var imageId = 0;
        try
        {
            var image = await _imageStorage.CreateImageInternal(request.File);

            if (image.Failed)
            {
                return Result.Failure<UpdateUserImageResponseDTO>(image.Error);
            }

            var insertImage = await _sender.Send(new InsertImageCommand
            {
                Image = image.Data
            });

            if (insertImage.Failed)
            {
                return Result.Failure<UpdateUserImageResponseDTO>(insertImage.Error);
            }

            imageId = insertImage.Data;

            var updateUserImage = await _sender.Send(new UpdateUserImageCommand
            {
                CurrentUserId = _currentUser.UserId,
                ImageId = insertImage.Data
            });

            if (updateUserImage.Failed)
            {
                await _sender.Send(new DeleteImageCommand
                {
                    ImageId = insertImage.Data
                });
                return Result.Failure<UpdateUserImageResponseDTO>(updateUserImage.Error);
            }

            var result = new UpdateUserImageResponseDTO
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
            return Result.Failure<UpdateUserImageResponseDTO>(
                new Error(ErrorType.Account, $"Error at {nameof(UpdateUserImage)}"));
        }
    }
}