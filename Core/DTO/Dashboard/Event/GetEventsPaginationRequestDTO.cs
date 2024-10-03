namespace How.Core.DTO.Dashboard.Event;

using Common.DTO;
using Infrastructure.Enums;

public sealed class GetEventsPaginationRequestDTO : PaginationDTO
{
    public string Search { get; set; } = string.Empty;
    public EventStatusFilter Status { get; set; }
    public EventAccessFilter Access { get; set; }
}