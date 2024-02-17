namespace How.Common.ResultClass;

public class Result<T> : Result
{
    private T _data;

    protected Result(T data)
    {
        Data = data;
    }

    public T Data
    {
        get => Success
            ? _data
            : throw new Exception($"You can't access .{nameof(Data)} when .{nameof(Success)} is false");
        set => _data = value;
    }
}