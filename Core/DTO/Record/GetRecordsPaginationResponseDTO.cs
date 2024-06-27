namespace How.Core.DTO.Record;

using Models;

public sealed class GetRecordsPaginationResponseDTO
{
    public int Count { get; set; }
    public ICollection<RecordItemModelDTO> Records { get; set; } = new List<RecordItemModelDTO>();
}