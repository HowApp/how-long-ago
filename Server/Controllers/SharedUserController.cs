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
}