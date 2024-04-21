namespace How.Common.Attributes;

using System.ComponentModel.DataAnnotations;
using Enums;
using Exceptions;
using Helpers;
using Microsoft.AspNetCore.Http;
using Validators;

public class FileValidatorAttribute : ValidationAttribute
{
    private readonly AppFileExt[] _allowedExtensions;
    private readonly long _maxSizeBytes;
     
    public FileValidatorAttribute(AppFileExt[] allowedExtensions, long maxSizeBytes)
    {
        _allowedExtensions = allowedExtensions;
        _maxSizeBytes = maxSizeBytes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file)
        {
            throw new FileValidationException(new Dictionary<string, string>()
            {
                {"File", "The Provided object is not a file."}
            });
        }

        if (file.Length == 0)
        {
            throw new FileValidationException(new Dictionary<string, string>()
            {
                {"FileContent", "The Provided file is empty."}
            });
        }
        
        if (file.Length > _maxSizeBytes)
        {
            throw new FileValidationException(new Dictionary<string, string>()
            {
                {"FileContent", $"Maximum allowed file size is {_maxSizeBytes / 1024L / 1024L} megabytes."}
            });
        }
        
        var extension = Path.GetExtension(file.FileName);

        if (!_allowedExtensions.Contains(AppFileTypeHelper.GetFileTypeFromExtensions(extension)))
        {
            throw new FileValidationException(new Dictionary<string, string>()
            {
                {"FileExtension", $"Extension: {extension} is not allowed to upload. " +
                                  $"We accept files with the following extensions: " +
                                  $"{AppFileTypeHelper.GetFileTypeFromExtensions(_allowedExtensions)}"}
            });
        }

        if (!FileSignatureValidator.IsValidImageExtensionAndSignature(file, _allowedExtensions))
        {
            throw new FileValidationException(new Dictionary<string, string>()
            {
                {"FileSignature", $"The Provided file has an invalid signature"}
            });
        }

        return ValidationResult.Success;
    }
}