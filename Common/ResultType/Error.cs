namespace How.Common.ResultType;

public class Error : Dictionary<string, string>
{
    public string ErrorType { get; }
    public string ErrorMessage { get; }
    
    public Error(string errorType, string errorMessage) : base(0)
    {
        ErrorType = errorType ?? throw new ArgumentNullException(nameof(errorType));
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        
        this.Add(ErrorType, ErrorMessage);
    }

    public override string ToString()
    {
        return string.Join(";\n", $"{ErrorType}: {ErrorMessage}");
    }
}