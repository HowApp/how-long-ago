namespace How.Core.DTO.Event;

using Models;

public sealed class GetEventsPaginationResponseDTO
{
    public int Count { get; set; }
    public EventItemModelDTO Events { get; set; }
}