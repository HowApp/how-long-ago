namespace How.Core.DTO.Public.Event;

using Models;

public sealed class GetEventsPaginationPublicResponseDTO
{
    public int Count { get; set; }
    public List<EventItemPublicModelDTO> Events { get; set; }
}