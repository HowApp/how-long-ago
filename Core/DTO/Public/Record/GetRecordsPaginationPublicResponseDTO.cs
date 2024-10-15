namespace How.Core.DTO.Public.Record;

using Models;

public sealed class GetRecordsPaginationPublicResponseDTO
{
    public int Count { get; set; }
    public ICollection<RecordItemPublicModelDTO> Records { get; set; } = new List<RecordItemPublicModelDTO>();
}