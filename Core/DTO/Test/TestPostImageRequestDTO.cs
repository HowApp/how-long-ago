namespace How.Core.DTO.Test;

using Common.Attributes;
using Common.Enums;
using Microsoft.AspNetCore.Http;

public sealed class TestPostImageRequestDTO
{
    [FileValidator(new []{ AppFileExt.JPG, AppFileExt.PNG, AppFileExt.JPEG, AppFileExt.WEBP}, 10L * 1024L * 1024L)]
    public IFormFile file { get; set; }
}