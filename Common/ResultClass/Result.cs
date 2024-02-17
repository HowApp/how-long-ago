namespace How.Common.ResultClass;

public abstract class Result
{
    public bool Success { get; protected set; }
    public bool Failure => !Success;
}