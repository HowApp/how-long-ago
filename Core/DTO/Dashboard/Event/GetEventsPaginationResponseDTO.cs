namespace How.Core.DTO.Dashboard.Event;

using Models;

public sealed class GetEventsPaginationResponseDTO
{
    public int Count { get; set; }
    public List<EventItemPrivateModelDTO> Events { get; set; }
}