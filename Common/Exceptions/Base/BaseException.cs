namespace How.Common.Exceptions.Base;

public class BaseException : Exception
{
    public Dictionary<string, string> Inconsistencies { get; }

    public BaseException(string message) : base(message)
    {
        Inconsistencies = new Dictionary<string, string>();
    }
    
    public BaseException(string message, Dictionary<string, string> inconsistencies) : base(message)
    {
        Inconsistencies = inconsistencies;
    }
}