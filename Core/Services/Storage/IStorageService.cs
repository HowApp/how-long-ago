namespace How.Core.Services.Storage
{
    using Common.ResultType;
    using Microsoft.AspNetCore.Http;
    using Models.ServicesModel.StorageService;

    public interface IStorageService
    {
        Task<Result> PostImageToDatabase(IFormFile file);
        Task<Result<GetImageFromDatabaseByteResult>> GetImageFromDatabaseByte(string fileHash);
        Task<Result<GetImageFromDatabaseStreamResult>> GetImageFromDatabaseStream(string fileHash);
    }
}
