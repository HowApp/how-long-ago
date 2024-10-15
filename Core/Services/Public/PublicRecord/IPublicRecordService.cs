namespace How.Core.Services.Public.PublicRecord;

using Common.ResultType;
using DTO.Public.Record;
using DTO.Record;

public interface IPublicRecordService
{
    Task<Result<GetRecordsPaginationPublicResponseDTO>> GetRecordsPagination(
        int eventId,
        GetRecordsPaginationRequestDTO request);
}