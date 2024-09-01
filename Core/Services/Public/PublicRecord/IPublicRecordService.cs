namespace How.Core.Services.Public.PublicRecord;

using Common.ResultType;
using DTO.Record;

public interface IPublicRecordService
{
    Task<Result<GetRecordsPaginationResponseDTO>> GetRecordsPagination(
        int eventId,
        GetRecordsPaginationRequestDTO request);
}