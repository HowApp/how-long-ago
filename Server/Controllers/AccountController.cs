namespace How.Server.Controllers;

using Core.DTO.Account;
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
    [ProducesResponseType<GetUserInfoResponseDTO>(200)]
    [Route("api/account/info")]
    public async Task<IActionResult> GetUserInfo()
    {
        var result = await _accountService.GetUserInfo();

        return HttpResult(result);
    }

    [HttpPatch]
    [SwaggerOperation("Update user info")]
    [ProducesResponseType<UpdateUserImageResponseDTO>(200)]
    [Route("api/account/info")]
    public async Task<IActionResult> UpdateUserInfo([FromQuery] UpdateUserInfoRequestDTO request)
    {
        var result = await _accountService.UpdateUserInfo(request);

        return HttpResult(result);
    }
    
    [HttpPut]
    [SwaggerOperation("Update user image, returning hash")]
    [ProducesResponseType<UpdateUserImageResponseDTO>(200)]
    [Route("api/account/image")]
    public async Task<IActionResult> UpdateUserImage([FromForm] UpdateUserImageRequestDTO request)
    {
        var result = await _accountService.UpdateUserImage(request);

        return HttpResult(result);
    }
}