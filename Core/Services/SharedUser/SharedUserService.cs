namespace How.Core.Services.SharedUser;

using Common.Extensions;
using Common.ResultType;
using CQRS.Commands.SharedUser.CreateSharedUser;
using CQRS.Queries.General.CheckExist;
using CQRS.Queries.General.CheckExistForUser;
using CurrentUser;
using Database;
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
}