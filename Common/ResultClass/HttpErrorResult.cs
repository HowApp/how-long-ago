namespace How.Common.ResultClass;

using System.Net;

public class HttpErrorResult : ErrorResult
{
    public HttpStatusCode StatusCode { get; }
    public HttpErrorResult(string message, IReadOnlyCollection<Error> errors, HttpStatusCode statusCode) : base(message, errors)
    {
        StatusCode = statusCode;
    }

    public HttpErrorResult(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}