namespace How.Server.Controllers.Identity;

using Common.Constants;
using Core.Services.Identity;
using Microsoft.AspNetCore.Mvc;
using Common.ResultType;
using Core.DTO.Identity;

[ApiExplorerSettings(GroupName = SwaggerDocConstants.Identity)]
public class IdentityController : BaseController
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [Route("api/identity/login")]
    [HttpPost]
    [ProducesResponseType(typeof(Result), 200)]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        var result = await _identityService.Login(request);

        return HttpResult(result);
    }

    [Route("api/identity/register")]
    [HttpPost]
    [ProducesResponseType(typeof(Result), 200)]
    public async Task<IActionResult> Register(RegisterRequestDTO request)
    {
        var result = await _identityService.Register(request);

        if (result.Failed)
        {
            return HttpResult(result);
        }

        return HttpResult(result);
    }

    [Route("api/identity/logout")]
    [HttpPost]
    [ProducesResponseType(typeof(Result), 200)]
    public async Task<IActionResult> Logout()
    {
        var result = await _identityService.Logout();

        return HttpResult(result);
    }
    
    [Route("api/identity/current-user-info")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<CurrentUserResponseDTO>), 200)]
    public IActionResult CurrentUserInfo()
    {
        var result = _identityService.GetCurrentUserInfo();

        return HttpResult(result);
    }
}