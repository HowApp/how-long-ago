namespace How.Server.Controllers.Public;

using Core.Services.Storage.FileStorage;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

public class StorageController : BaseController
{
    private readonly IFileStorageService _fileStorageService;

    public StorageController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
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

