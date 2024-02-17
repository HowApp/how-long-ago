namespace How.Server.Controllers;

using System.Net;
using Common.ResultClass;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult HttpResult(Result resultIncome)
    {
        if (resultIncome.Success)
        {
            return Ok();
        }

        if (resultIncome is ErrorResult errorResult)
        {
            var result = new HttpErrorResult(errorResult.Message, errorResult.Errors, HttpStatusCode.BadRequest);
            return StatusCode((int)HttpStatusCode.BadRequest, result);
        }
        
        return StatusCode((int)HttpStatusCode.BadRequest);
    }
    
    protected IActionResult HttpResult<T>(Result<T> resultIncome)
    {
        if (resultIncome.Success)
        {
            return Ok(resultIncome);
        }

        if (resultIncome is ErrorResult<T> errorResult)
        {
            var result = new HttpErrorResult(errorResult.Message, errorResult.Errors, HttpStatusCode.BadRequest);
            return StatusCode((int)HttpStatusCode.BadRequest, result);
        }
        
        return StatusCode((int)HttpStatusCode.BadRequest);
    }
}