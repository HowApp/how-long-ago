namespace How.Core.Services.Storage.FileStorage
{
    using Common.ResultType;
    using Microsoft.AspNetCore.Http;
    using Models.ServicesModel.StorageService;

    public interface IFileStorageService
    {
        Task<Result> PostFileToDatabase(IFormFile file);
        Task<Result<GetImageFromDatabaseByteResult>> GetFileFromDatabaseByte(string fileHash);
        Task<Result<GetImageFromDatabaseStreamResult>> GetFileFromDatabaseStream(string fileHash);
    }
}
