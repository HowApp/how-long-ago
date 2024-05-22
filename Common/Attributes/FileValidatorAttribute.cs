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
    private readonly int _maxNumbers;
     
    public FileValidatorAttribute(AppFileExt[] allowedExtensions, long maxSizeBytes, int maxNumbers = 1)
    {
        _allowedExtensions = allowedExtensions;
        _maxSizeBytes = maxSizeBytes;
        _maxNumbers = maxNumbers;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            throw new FileValidationException(new Dictionary<string, string>()
            {
                {"File", "Empty file."}
            });
        }

        if (value is IFormFile file)
        {
            ValidateFile(file);
        }
        else if (value is IFormFileCollection files)
        {
            if (files.Count > _maxNumbers)
            {
                throw new FileValidationException(new Dictionary<string, string>()
                {
                    {"File", $"Allowed to upload only {_maxNumbers} items."}
                });
            }
            
            foreach (var item in files)
            {
                ValidateFile(item);
            }
        }
        else
        {
            throw new FileValidationException(new Dictionary<string, string>()
            {
                {"File", "The Provided object is not a file or file collection."}
            });
        }
        
        return ValidationResult.Success;
    }

    private void ValidateFile(IFormFile file)
    {
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
    }
}