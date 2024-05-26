namespace How.Core.Services.Record;

using Common.ResultType;
using DTO.Record;
using DTO.RecordImage;

public interface IRecordService
{
    Task<Result<int>> CreateRecord(int eventId, CreateRecordRequestDTO request);
    Task<Result> UpdateRecord(int recordId, UpdateRecordRequestDTO request);
    Task<Result<CreateRecordImagesResponseDTO>> CreateRecordImages(int recordId, CreateRecordImagesRequestDTO request);
    Task<Result> UpdateRecordImages(int recordId, UpdateRecordImagesRequestDTO request);
}