namespace How.Core.Services.Record;

using Common.ResultType;
using DTO.Record;
using DTO.RecordImage;

public interface IRecordService
{
    Task<Result<int>> CreateRecord(int eventId, CreateRecordRequestDTO request);
    Task<Result> UpdateRecord(int eventId, int recordId, UpdateRecordRequestDTO request);
    Task<Result<CreateRecordImagesResponseDTO>> CreateRecordImage(int eventId, int recordId, CreateRecordImagesRequestDTO request);
}