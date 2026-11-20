namespace How.Server.Controllers;

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Common.ResultType;

[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult HttpResult(Result result)
    {
        if (result.Succeeded)
        {
            return Ok(result);
        }

        var statusCode = result.Code >= 400 ? result.Code : (int)HttpStatusCode.BadRequest;
        return StatusCode(statusCode, result);
    }

    protected IActionResult HttpResult<T>(Result<T> result)
    {
        if (result.Succeeded)
        {
            return Ok(result);
        }

        var statusCode = result.Code >= 400 ? result.Code : (int)HttpStatusCode.BadRequest;
        return StatusCode(statusCode, result);
    }
}