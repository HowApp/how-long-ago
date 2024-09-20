namespace How.Core.Services.Record;

using Common.ResultType;
using DTO.Record;
using DTO.RecordImage;
using Infrastructure.Enums;

public interface IRecordService
{
    Task<Result<int>> CreateRecord(
        int eventId,
        CreateRecordRequestDTO request);
    Task<Result<GetRecordsPaginationResponseDTO>> GetRecordsPagination(
        int eventId,
        GetRecordsPaginationRequestDTO request);
    Task<Result> UpdateRecord(
        int eventId,
        int recordId,
        UpdateRecordRequestDTO request);
    Task<Result<LikeState>> UpdateLikeState(
        int eventId,
        int recordId,
        LikeState likeState);
    Task<Result> CreateRecordImages(
        int eventId,
        int recordId,
        CreateRecordImagesRequestDTO request);
    Task<Result> UpdateRecordImages(
        int eventId,
        int recordId,
        UpdateRecordImagesRequestDTO request);
    Task<Result> DeleteRecord(
        int eventId,
        int recordId);
}