namespace How.Core.Services.Storage.ImageStorage;

using Common.ResultType;
using Microsoft.AspNetCore.Http;
using Models.ServicesModel;

public interface IImageStorageService
{
    Task<Result> PostImageToDatabase(IFormFile file);
    Task<Result<ImageInternalModel>> CreateImageInternal(IFormFile file);
    Task<Result<ImageInternalModel>> CreateImageInternal(byte[] file, string fileName);
}