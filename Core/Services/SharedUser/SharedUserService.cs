namespace How.Core.Services.SharedUser;

using Common.Extensions;
using Common.ResultType;
using CQRS.Commands.SharedUser.CreateSharedUser;
using CQRS.Queries.General.CheckExist;
using CQRS.Queries.SharedUser.GetSharedUsers;
using CurrentUser;
using Database;
using DTO.Models;
using DTO.SharedUser;
using MediatR;
using Microsoft.Extensions.Logging;

public class SharedUserService : ISharedUserService
{
    private readonly ILogger<SharedUserService> _logger;
    private readonly ISender _sender;
    private readonly ICurrentUserService _userService;

    public SharedUserService(ILogger<SharedUserService> logger, ISender sender, ICurrentUserService userService)
    {
        _logger = logger;
        _sender = sender;
        _userService = userService;
    }

    public async Task<Result<int>> CreateSharedUser(CreateSharedUserRequestDTO request)
    {
        try
        {
            if (request.UserId == _userService.UserId)
            {
                return Result.Failure<int>(
                    new Error(ErrorType.Record, $"You can't perform this operation to yourself"), 418);
            }
            
            var sharedUserExist = await _sender.Send(new CheckExistQuery
            {
                Id = request.UserId,
                Table = nameof(BaseDbContext.Users).ToSnake()
            });

            if (sharedUserExist.Failed)
            {
                return Result.Failure<int>(sharedUserExist.Error);
            }

            if (!sharedUserExist.Data)
            {
                return Result.Failure<int>(
                    new Error(ErrorType.Record, $"User not found!"), 404);
            }
            
            var command = new CreateSharedUserCommand
            {
                CurrentUserId = _userService.UserId,
                SharedUserId = request.UserId
            };

            var result = await _sender.Send(command);

            if (result.Failed)
            {
                return Result.Failure<int>(result.Error);
            }

            if (result.Data < 1)
            {
                return Result.Failure<int>(
                    new Error(ErrorType.SharedUser, $"Shared User not created!"));
            }
            
            return Result.Success(result.Data);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.SharedUser, $"Error at {nameof(CreateSharedUser)}"));
        }
    }

    public async Task<Result<GetSharedUsersResponseDTO>> GetSharedUsers()
    {
        try
        {
            var query = new GetSharedUsersQuery
            {
                CurrentUserId = _userService.UserId
            };

            var queryResult = await _sender.Send(query);

            if (queryResult.Failed)
            {
                return Result.Failure<GetSharedUsersResponseDTO>(queryResult.Error);
            }

            var users = queryResult.Data.Select(r => new UserInfoModelLongDTO
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                UserName = r.UserName,
                Image = new ImageModelDTO
                {
                    MainHash = r.MainHash,
                    ThumbnailHash = r.ThumbnailHash
                }
            }).ToList();

            var result = new GetSharedUsersResponseDTO
            {
                Users = users
            };

            return new Result<GetSharedUsersResponseDTO>(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetSharedUsersResponseDTO>(
                new Error(ErrorType.SharedUser, $"Error at {nameof(GetSharedUsers)}"));
        }
    }
}