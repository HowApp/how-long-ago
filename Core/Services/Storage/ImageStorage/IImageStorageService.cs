namespace How.Core.Services.Storage.ImageStorage;

using Common.ResultType;
using Microsoft.AspNetCore.Http;

public interface IImageStorageService
{
    Task<Result> PostImageToDatabase(IFormFile file);
}