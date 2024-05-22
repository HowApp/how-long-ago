namespace How.Core.Services.Record;

using Common.ResultType;
using DTO.Record;

public interface IRecordService
{
    Task<Result<int>> CreateRecord(int eventId, CreateRecordRequestDTO request);
    Task<Result> UpdateRecord(int eventId, int recordId, UpdateRecordRequestDTO request);
}