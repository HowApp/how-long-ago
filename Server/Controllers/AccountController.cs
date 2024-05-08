namespace How.Server.Controllers;

using Core.Services.Account;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class AccountController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
}