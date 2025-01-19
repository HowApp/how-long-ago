namespace How.Core.Services.Identity;

using Microsoft.Extensions.Logging;
using Common.ResultType;
using CurrentUser;
using DTO.Identity;

public class IdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public IdentityService(
        ILogger<IdentityService> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public Result<CurrentUserResponseDTO> GetCurrentUserInfo()
    {
        try
        {
            var user = _currentUserService.User;

            if (user.Identity is null)
            {
                return Result.Failure<CurrentUserResponseDTO>(new Error(
                    ErrorType.Account,
                    "User not found!"), 404);
            }

            var result = new CurrentUserResponseDTO
            {
                IsAuthenticate = user.Identity.IsAuthenticated,
                UserName = user.Identity.Name ?? string.Empty,
                Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value)
            };

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<CurrentUserResponseDTO>(new Error(
                ErrorType.Account,
                $"Error while executing {nameof(GetCurrentUserInfo)}!"));
        }
    }
}