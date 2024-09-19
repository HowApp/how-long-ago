namespace How.Common.ResultType;

public class Result
{
    public bool Succeeded { get; }
    public bool Failed => !Succeeded;
    public int Code { get; }
    public Error? Error { get; }
    
    public Result(Error? error, int code = 400)
    {
        Succeeded = false;
        Error = error;
        Code = code;
    }

    public Result(int code = 200)
    {
        Succeeded = true;
        Code = code;
    }

    public static Result Success() => new();
    public static Result Failure(Error? error, int code = 400) => new(error, code);
    public static Result<TData> Success<TData>(TData data, int code = 200) => new(data, code);
    public static Result<TData> Failure<TData>(Error? error, int code = 400) => new(error, code);
    
    public string GetErrorMessages()
    {
        if (Succeeded)
        {
            return "Success";
        }
        if (Error is not null)
        {
            return Error.ToString();
        }

        return "unknown error";
    }
}

public class Result<TData> : Result
{
    private TData? _data;

    public TData Data
    {
        get => Succeeded ? _data! : default!;
        set => _data = value;
    }

    public Result(Error? error, int code = 400) : base(error, code)
    {
    }
    
    public Result(TData data, int code = 200) : base(code)
    {
        Data = data;
    }
}