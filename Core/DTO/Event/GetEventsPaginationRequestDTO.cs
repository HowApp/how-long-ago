namespace How.Core.DTO.Event;

using Common.DTO;
using Infrastructure.Enums;

public sealed class GetEventsPaginationRequestDTO : PaginationDTO
{
    public string Search { get; set; } = string.Empty;
    public EventStatus Status { get; set; }
}