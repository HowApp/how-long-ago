namespace How.Server.Controllers;

using Common.ResultType;
using Core.DTO.SharedUser;
using Core.Services.SharedUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Authorize]
public class SharedUserController : BaseController
{
    private readonly ISharedUserService _userService;

    public SharedUserController(ISharedUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    [SwaggerOperation("Create shared User, return ID")]
    [ProducesResponseType<Result<int>>(200)]
    [Route("api/shared-user/create")]
    public async Task<IActionResult> CreateSharedUser([FromBody] CreateSharedUserRequestDTO request)
    {
        var result = await _userService.CreateSharedUser(request);

        return HttpResult(result);
    }

    [HttpGet]
    [SwaggerOperation("Get shared Users")]
    [ProducesResponseType<Result<GetSharedUsersResponseDTO>>(200)]
    [Route("api/shared-user/get")]
    public async Task<IActionResult> GetSharedUsers()
    {
        var result = await _userService.GetSharedUsers();

        return HttpResult(result);
    }
    
    [HttpDelete]
    [SwaggerOperation("Delete shared Users")]
    [ProducesResponseType<Result>(200)]
    [Route("api/shared-user/delete")]
    public async Task<IActionResult> DeleteSharedUser([FromBody] DeleteSharedUserRequestDTO request)
    {
        var result = await _userService.DeleteSharedUser(request);

        return HttpResult(result);
    }
}