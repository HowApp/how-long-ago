namespace How.Common.ResultType;

public class Result
{
    public bool Succeeded { get; }
    public bool Failed => !Succeeded;
    public Error? Error { get; }
    
    public Result(Error error)
    {
        Succeeded = false;
        Error = error;
    }
    
    public Result()
    {
        Succeeded = true;
    }

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);
    public static Result<TData> Success<TData>(TData data) => new(data);
    public static Result<TData> Failure<TData>(Error error) => new(error);
}

public class Result<TData> : Result
{
    private TData? _data;

    public TData Data
    {
        get => Succeeded ? _data! : default!;
        set => _data = value;
    }

    public Result(Error error) : base(error)
    {
    }
    
    public Result(TData data) : base()
    {
        Data = data;
    }
}