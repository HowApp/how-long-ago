namespace How.Server.Controllers;

using Core.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Authorize]
public class AccountController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpGet]
    [SwaggerOperation("Get user info")]
    [Route("api/account/info")]
    public async Task<IActionResult> GetUserInfo()
    {
        var result = await _accountService.GetUserInfo();

        return HttpResult(result);
    }
}