namespace How.Core.Services.Account;

using Common.ResultType;
using CQRS.Queries.Account.GetUserInfo;
using CurrentUser;
using DTO.Account;
using DTO.Models;
using MediatR;
using Microsoft.Extensions.Logging;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public AccountService(ISender sender, ICurrentUserService currentUser, ILogger<AccountService> logger)
    {
        _sender = sender;
        _currentUser = currentUser;
        _logger = logger;
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
                Image = new ImageModel
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
}