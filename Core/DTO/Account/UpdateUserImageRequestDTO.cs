namespace How.Core.DTO.Account;

using Common.Attributes;
using Common.Enums;
using Microsoft.AspNetCore.Http;

public sealed class UpdateUserImageRequestDTO
{
    [FileValidator(
        new AppFileExt[] { AppFileExt.JPEG, AppFileExt.JPG, AppFileExt.PNG, AppFileExt.WEBP },
        10 * 1024 * 1024)]
    public IFormFile File { get; set; }
}