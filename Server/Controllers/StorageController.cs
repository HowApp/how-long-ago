namespace How.Server.Controllers;

using Core.DTO.Storage.FileService;
using Core.DTO.Storage.ImageService;
using Core.Services.Storage.FileStorage;
using Core.Services.Storage.ImageStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Authorize]
public class StorageController : BaseController
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IImageStorageService _imageStorageService;

    public StorageController(IFileStorageService fileStorageService, IImageStorageService imageStorageService)
    {
        _fileStorageService = fileStorageService;
        _imageStorageService = imageStorageService;
    }

    [HttpPost]
    [SwaggerOperation("Upload file to server")]
    [Route("api/storage/file/create")]
    public async Task<IActionResult> PostFile(CreateFileRequestDTO request)
    {
        var result = await _fileStorageService.PostFileToDatabase(request.File);

        return HttpResult(result);
    }
    
    [HttpPost]
    [SwaggerOperation("Upload image to server")]
    [Route("api/storage/image/create")]
    public async Task<IActionResult> PostImage(CreateImageRequestDTO request)
    {
        var result = await _imageStorageService.PostImageToDatabase(request.File);

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get file byte")]
    [Route("api/storage/file/{fileHash:required}/get-byte")]
    public async Task<IActionResult> GetByte([FromRoute] string fileHash)
    {
        var result = await _fileStorageService.GetFileFromDatabaseByte(fileHash);

        return File(result.Data.Content, result.Data.MimeType, result.Data.FileName);
    }
    
    [HttpGet]
    [SwaggerOperation("Get file stream")]
    [Route("api/storage/file/{fileHash:required}/get-stream")]
    public async Task<IActionResult> GetStream([FromRoute] string fileHash)
    {
        var result = await _fileStorageService.GetFileFromDatabaseStream(fileHash);

        return new FileStreamResult(result.Data.Content, result.Data.MimeType);
    }
}

