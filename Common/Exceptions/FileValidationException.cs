namespace How.Common.Exceptions;

using Base;

public class FileValidationException : BaseException
{
    public FileValidationException() : base("Inconsistencies while file validation!")
    {
    }
    
    public FileValidationException(Dictionary<string, string> inconsistencies) 
        : base("Inconsistencies while file validation!", inconsistencies)
    {
    }
}