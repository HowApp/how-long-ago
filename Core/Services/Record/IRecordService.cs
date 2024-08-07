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
        FilterType filterType = FilterType.IncludeCreatedBy);
    Task<Result<GetRecordsPaginationResponseDTO>> GetRecordsPagination(
        int eventId,
        GetRecordsPaginationRequestDTO request,
        FilterType filterType = FilterType.IncludeCreatedBy);
    Task<Result> UpdateRecord(
        int recordId,
        UpdateRecordRequestDTO request,
        FilterType filterType = FilterType.IncludeCreatedBy);
    Task<Result<CreateRecordImagesResponseDTO>> CreateRecordImages(
        int recordId,
        CreateRecordImagesRequestDTO request,
        FilterType filterType = FilterType.IncludeCreatedBy);
    Task<Result> UpdateRecordImages(
        int recordId,
        UpdateRecordImagesRequestDTO request,
        FilterType filterType = FilterType.IncludeCreatedBy);
    Task<Result> DeleteRecord(
        int recordId,
        FilterType filterType = FilterType.IncludeCreatedBy);
}