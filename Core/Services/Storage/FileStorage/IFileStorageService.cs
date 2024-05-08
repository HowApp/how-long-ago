namespace How.Core.Services.Storage.FileStorage;

using Common.ResultType;
using DTO.Storage.FileService;
using Microsoft.AspNetCore.Http;

public interface IFileStorageService
{
    Task<Result> PostFileToDatabase(IFormFile file);
    Task<Result<GetFileFromDatabaseByteResponseDTO>> GetFileFromDatabaseByte(string fileHash);
    Task<Result<GetFileFromDatabaseStreamResponseDTO>> GetFileFromDatabaseStream(string fileHash);
}