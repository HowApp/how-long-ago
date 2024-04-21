namespace How.Core.Services.AccountServices;

using Common.Constants;
using Common.Extensions;
using Database.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Models.ServicesModel.AccountService;
using Common.ResultType;
using UserServices;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly UserManager<HowUser> _userManager;
    private readonly SignInManager<HowUser> _signInManager;
    private readonly IUserService _userService;

    public AccountService(
        ILogger<AccountService> logger,
        UserManager<HowUser> userManager, 
        SignInManager<HowUser> signInManager, 
        IUserService userService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    public async Task<Result> Login(LoginRequestModel requestModel)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(requestModel.Email);

            if (user is null)
            {
                return Result.Failure(new Error(ErrorType.Account, "User not found!"));
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

    public async Task<Result> Register(RegisterRequestModel requestModel)
    {
        try
        {
            var user = new HowUser
            {
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

    public async Task<Result<CurrentUserResponseModel>> GetCurrentUserInfo()
    {
        try
        {
            var user = _userService.User;

            if (user.Identity is null)
            {
                return Result.Failure<CurrentUserResponseModel>(new Error(
                    ErrorType.Account,
                    "User not found!"));
            }

            var result = new CurrentUserResponseModel
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
            return Result.Failure<CurrentUserResponseModel>(new Error(
                ErrorType.Account,
                $"Error while executing {nameof(GetCurrentUserInfo)}!"));
        }
    }
}

