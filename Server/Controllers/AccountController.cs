namespace How.Server.Controllers;

using Common.ResultType;
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
    [SwaggerOperation("Get current user info")]
    [ProducesResponseType<Result<GetUserInfoResponseDTO>>(200)]
    [Route("api/account/info/current")]
    public async Task<IActionResult> GetUserInfo()
    {
        var result = await _accountService.GetUserInfo();

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get user info by User Name")]
    [ProducesResponseType<Result<GetUserInfoByUserNameResponseDTO>>(200)]
    [Route("api/account/info/by-user-name")]
    public async Task<IActionResult> GetUserInfoByUserName([FromQuery] GetUserInfoByUserNameRequestDTO request)
    {
        var result = await _accountService.GetUserInfoByUserName(request);

        return HttpResult(result);
    }

    [HttpPatch]
    [SwaggerOperation("Update user info")]
    [ProducesResponseType<Result>(200)]
    [Route("api/account/info")]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoRequestDTO request)
    {
        var result = await _accountService.UpdateUserInfo(request);

        return HttpResult(result);
    }
    
    [HttpPut]
    [SwaggerOperation("Update user image, returning hash")]
    [ProducesResponseType<Result<UpdateUserImageResponseDTO>>(200)]
    [Route("api/account/image")]
    public async Task<IActionResult> UpdateUserImage([FromForm] UpdateUserImageRequestDTO request)
    {
        var result = await _accountService.UpdateUserImage(request);

        return HttpResult(result);
    }
}