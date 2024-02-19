namespace How.Server.Controllers;

using System.Net;
// using Common.ResultClass;
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
        
        return StatusCode((int)HttpStatusCode.BadRequest, result);
    }
    
    protected IActionResult HttpResult<T>(Result<T> result)
    {
        if (result.Succeeded)
        {
            return Ok(result);
        }
        
        return StatusCode((int)HttpStatusCode.BadRequest, result);
    }
}