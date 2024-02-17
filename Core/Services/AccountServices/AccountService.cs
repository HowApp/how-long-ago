namespace How.Core.Services.AccountServices;

using Common.ResultClass;
using Database.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Models.ServicesModel.AccountService;
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
                return new ErrorResult("User not found!");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user, 
                requestModel.Password, 
                false);

            if (!signInResult.Succeeded)
            {
                return new ErrorResult("Invalid Password!");
            }

            await _signInManager.SignInAsync(user, requestModel.RememberMe);

            return new SuccessResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new ErrorResult($"Error while executing {nameof(Login)}!");
        }
    }

    public async Task<Result<RegisterResponseModel>> Register(RegisterRequestModel requestModel)
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
                return ErrorResultExtensions.FromIdentityErrors<RegisterResponseModel>(
                    "Error while creating new User!", result.Errors);
            }

            return new SuccessResult<RegisterResponseModel>(new RegisterResponseModel
            {
                Email = requestModel.UserName,
                Password = requestModel.Email,
                RememberMe = false
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new ErrorResult<RegisterResponseModel>($"Error while executing {nameof(Register)}!");
        }
    }

    public async Task<Result> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();

            return new SuccessResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new ErrorResult($"Error while executing {nameof(Logout)}!");
        }
    }

    public async Task<Result<CurrentUserResponseModel>> GetCurrentUserInfo()
    {
        try
        {
            var user = _userService.User;

            if (user.Identity is null)
            {
                return new ErrorResult<CurrentUserResponseModel>("User not found!");
            }

            var result = new CurrentUserResponseModel
            {
                IsAuthenticate = user.Identity.IsAuthenticated,
                UserName = user.Identity.Name ?? string.Empty,
                Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value)
            };

            return new SuccessResult<CurrentUserResponseModel>(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new ErrorResult<CurrentUserResponseModel>(
                $"Error while executing {nameof(GetCurrentUserInfo)}!");
        }
    }
}

