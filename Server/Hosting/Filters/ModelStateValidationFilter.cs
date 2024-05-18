namespace How.Server.Hosting.Filters;

using Common.ResultType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ModelStateValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var error = new Error(ErrorType.Validation, "Model validation errors occurred during processing!");

            var modelState = context.ModelState;

            foreach (var (key, value) in modelState)
            {
                if (value.Errors.Any())
                {
                    error.Add(key, value.Errors.Select(e => e.ErrorMessage));
                }
            }

            context.Result = new BadRequestObjectResult(Result.Failure(error));
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}