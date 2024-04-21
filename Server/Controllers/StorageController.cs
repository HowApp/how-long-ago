namespace How.Server.Controllers;

using Core.DTO.Storage;
using Core.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

public class StorageController : BaseController
{
    private readonly IStorageService _storageService;

    public StorageController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
    [SwaggerOperation("Upload image to server")]
    [Route("api/storage/image/create")]
    public async Task<IActionResult> Create(CreateImageRequestDTO request)
    {
        var result = await _storageService.PostImageToDatabase(request.File);

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get image byte")]
    [Route("api/storage/image/{fileHash:required}/get-byte")]
    public async Task<IActionResult> GetByte([FromRoute] string fileHash)
    {
        var result = await _storageService.GetImageFromDatabaseByte(fileHash);

        return File(result.Data.Content, result.Data.MimeType, result.Data.FileName);
    }
    
    [HttpGet]
    [SwaggerOperation("Get image stream")]
    [Route("api/storage/image/{fileHash:required}/get-stream")]
    public async Task<IActionResult> GetStream([FromRoute] string fileHash)
    {
        var result = await _storageService.GetImageFromDatabaseStream(fileHash);

        return new FileStreamResult(result.Data.Content, result.Data.MimeType);
    }
}

