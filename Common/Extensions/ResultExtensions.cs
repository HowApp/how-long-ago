namespace How.Common.Extensions;

using Exceptions.Base;
using Microsoft.AspNetCore.Identity;
using ResultType;

public class ResultExtensions
{
    public static Result FromIdentityErrors(string message, IEnumerable<IdentityError> errors)
    {
        var error = new Error(ErrorType.Account, message);
        
        foreach (var e in errors)
        {
            error.Add(e.Code, e.Description);
        }

        return Result.Failure(error);
    }
    
    public static Result<T> FromIdentityErrors<T>(string message, IEnumerable<IdentityError> errors)
    {
        var error = new Error(ErrorType.Account, message);
        
        foreach (var e in errors)
        {
            error.Add(e.Code, e.Description);
        }

        return Result.Failure<T>(error);
    }
    
    public static Result FromAppException(BaseException exception)
    {
        var error = new Error(ErrorType.FileValidation, exception.Message);
        
        foreach (var (key, value) in exception.Inconsistencies)
        {
            error.Add(key, value);
        }

        return Result.Failure(error);
    }
}