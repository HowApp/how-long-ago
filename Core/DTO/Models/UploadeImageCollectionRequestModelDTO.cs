namespace How.Core.DTO.Models;

using Common.Attributes;
using Common.Enums;
using Microsoft.AspNetCore.Http;

public class UploadeImageCollectionRequestModelDTO
{
    [FileValidator(new AppFileExt[] { AppFileExt.JPEG, AppFileExt.JPG, AppFileExt.PNG, AppFileExt.WEBP }, 10 * 1024 * 1024, 5)]
    public IFormFileCollection Files { get; set; }
}