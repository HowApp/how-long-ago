namespace How.Server.Hosting.Filters;

using System.Net;
using Common.Exceptions.Base;
using Common.Extensions;
using Common.ResultType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var result = new Result();
        
        switch (context.Exception)
        {
            case BaseException fileTypeException:
                result = ResultExtensions.FromAppException(fileTypeException);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            default:
                result = Result.Failure(new Error(ErrorType.UnexpectedError, "Unexpected error..."));
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                
                _logger.LogError($"{context.Exception}");
                break;
        }
        
        context.Result = new JsonResult(result);
    }
}