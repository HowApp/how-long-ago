namespace How.Common.ResultClass;

public class ErrorResult : Result, IErrorResult
{
    public string Message { get; }
    public IReadOnlyCollection<Error> Errors { get; }

    public ErrorResult(string message, IReadOnlyCollection<Error> errors)
    {
        Message = message;
        Success = false;
        Errors = errors ?? Array.Empty<Error>();
    }

    public ErrorResult(string message) : this(message, Array.Empty<Error>())
    {
    }
}