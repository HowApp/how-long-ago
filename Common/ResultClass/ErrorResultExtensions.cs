namespace How.Common.ResultClass;

using Microsoft.AspNetCore.Identity;

public static class ErrorResultExtensions
{
    public static ErrorResult FromIdentityErrors(
        string message,
        IEnumerable<IdentityError> errors)
    {
        var errorInfo = errors
            .Select(error => new Error(error.Code, error.Description))
            .ToList();

        return new ErrorResult(message, errorInfo);
    }
    
    public static ErrorResult<T> FromIdentityErrors<T>(
        string message,
        IEnumerable<IdentityError> errors)
    {
        var errorInfo = errors
            .Select(error => new Error(error.Code, error.Description))
            .ToList();

        return new ErrorResult<T>(message, errorInfo);
    }
}