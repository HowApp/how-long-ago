namespace How.Core.Services.Identity;

using Common.Constants;
using Common.Extensions;
using Database.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Common.ResultType;
using CurrentUser;
using DTO.Identity;

public class IdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    private readonly UserManager<HowUser> _userManager;
    private readonly SignInManager<HowUser> _signInManager;
    private readonly ICurrentUserService _currentUserService;

    public IdentityService(
        ILogger<IdentityService> logger,
        UserManager<HowUser> userManager, 
        SignInManager<HowUser> signInManager, 
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Login(LoginRequestDTO requestModel)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(requestModel.Email);

            if (user is null)
            {
                return Result.Failure(new Error(ErrorType.Account, "User not found!"), 404);
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user, 
                requestModel.Password, 
                false);

            if (!signInResult.Succeeded)
            {
                return Result.Failure(new Error(ErrorType.Account,"Invalid Password!"));
            }
            
            await _signInManager.SignInAsync(user, requestModel.RememberMe);

            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(new Error(
                ErrorType.Account,
                $"Error while executing {nameof(Login)}!"));
        }
    }

    public async Task<Result> Register(RegisterRequestDTO requestModel)
    {
        try
        {
            var user = new HowUser
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                UserName = requestModel.UserName,
                Email = requestModel.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, requestModel.Password);

            if (!result.Succeeded)
            {
                return ResultExtensions.FromIdentityErrors(
                    "Error while creating new User!", result.Errors);
            }

            var addRoleToUser = await _userManager.AddToRoleAsync(user, AppConstants.Role.User.Name);

            if (!addRoleToUser.Succeeded)
            {
                return ResultExtensions.FromIdentityErrors(
                    "Error while adding role to User!", addRoleToUser.Errors);
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(new Error(
                ErrorType.Account,
                $"Error while executing {nameof(Register)}!"));
        }
    }

    public async Task<Result> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();

            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(new Error(
                ErrorType.Account,
                $"Error while executing {nameof(Logout)}!"));
        }
    }

    public async Task<Result<CurrentUserResponseDTO>> GetCurrentUserInfo()
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

