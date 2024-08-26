namespace How.Core.Services.Record;

using Common.ResultType;
using DTO.Record;
using DTO.RecordImage;
using Infrastructure.Enums;

public interface IRecordService
{
    Task<Result<int>> CreateRecord(
        int eventId,
        CreateRecordRequestDTO request,
        AccessFilterType accessFilterType = AccessFilterType.IncludeCreatedBy);
    Task<Result<GetRecordsPaginationResponseDTO>> GetRecordsPagination(
        int eventId,
        GetRecordsPaginationRequestDTO request,
        AccessFilterType accessFilterType = AccessFilterType.IncludeCreatedBy);
    Task<Result> UpdateRecord(
        int eventId,
        int recordId,
        UpdateRecordRequestDTO request,
        AccessFilterType accessFilterType = AccessFilterType.IncludeCreatedBy);
    Task<Result<LikeState>> UpdateLikeState(
        int eventId,
        int recordId,
        LikeState likeState);
    Task<Result<CreateRecordImagesResponseDTO>> CreateRecordImages(
        int eventId,
        int recordId,
        CreateRecordImagesRequestDTO request,
        AccessFilterType accessFilterType = AccessFilterType.IncludeCreatedBy);
    Task<Result> UpdateRecordImages(
        int eventId,
        int recordId,
        UpdateRecordImagesRequestDTO request,
        AccessFilterType accessFilterType = AccessFilterType.IncludeCreatedBy);
    Task<Result> DeleteRecord(
        int eventId,
        int recordId,
        AccessFilterType accessFilterType = AccessFilterType.IncludeCreatedBy);
}