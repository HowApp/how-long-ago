namespace How.Common.Exceptions;

using Base;

public class FileTypeException : BaseException
{
    public FileTypeException() : base("Missing file type error!")
    {
    }
    
    public FileTypeException(Dictionary<string, string> inconsistencies) 
        : base("Missing file type error!", inconsistencies)
    {
    }
}