namespace How.Core.DTO.Storage.FileService;

using Common.Attributes;
using Common.Enums;
using Microsoft.AspNetCore.Http;

public sealed class CreateFileRequestDTO
{
    [FileValidator(new AppFileExt[] { AppFileExt.JPEG, AppFileExt.JPG }, 10 * 1024 * 1024)]
    public IFormFile File { get; set; }
}
