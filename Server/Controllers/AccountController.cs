namespace How.Server.Controllers;

using Common.ResultClass;
using Core.Models.ServicesModel.AccountService;
using Core.Services.AccountServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Auth;

[Route("api/[controller]/[action]")]
public class AccountController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        var model = new LoginRequestModel
        {
            Email = request.Email,
            Password = request.Password,
            RememberMe = request.RememberMe
        };
        
        var result = await _accountService.Login(model);

        return HttpResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequestDTO request)
    {
        var model = new RegisterRequestModel
        {
            Email = request.Email,
            UserName = request.UserName,
            Password = request.Password
        };

        var result = await _accountService.Register(model);

        if (result.Failure)
        {
            return HttpResult(result);
        }

        var httpResult = new SuccessResult<RegisterResponseDTO>(new RegisterResponseDTO
        {
            Email = result.Data.Email,
            Password = result.Data.Password
        });

        return HttpResult(httpResult);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        var result = await _accountService.Logout();

        return HttpResult(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CurrentUserInfo()
    {
        var result = await _accountService.GetCurrentUserInfo();

        if (result.Failure)
        {
            return HttpResult(result);
        }

        var httpResult = new SuccessResult<CurrentUserResponseDTO>(new CurrentUserResponseDTO
        {
            IsAuthenticate = result.Data.IsAuthenticate,
            UserName = result.Data.UserName,
            Claims = result.Data.Claims
        });
        
        return HttpResult(httpResult);
    }
}