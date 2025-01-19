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

    [Route("api/identity/current-user-info")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<CurrentUserResponseDTO>), 200)]
    public IActionResult CurrentUserInfo()
    {
        var result = _identityService.GetCurrentUserInfo();

        return HttpResult(result);
    }
}