namespace How.Common.Extensions;

using Microsoft.AspNetCore.Identity;
using ResultType;

public class FailedResultExtensions
{
    public static Result FromIdentityErrors(
        string message,
        IEnumerable<IdentityError> errors)
    {
        var error = new Error(ErrorType.Account, message);
        
        foreach (var e in errors)
        {
            error.Add(e.Code, e.Description);
        }

        return Result.Failure(error);
    }
    
    public static Result<T> FromIdentityErrors<T>(
        string message,
        IEnumerable<IdentityError> errors)
    {
        var error = new Error(ErrorType.Account, message);
        
        foreach (var e in errors)
        {
            error.Add(e.Code, e.Description);
        }

        return Result.Failure<T>(error);
    }
}