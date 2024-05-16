namespace How.Core.DTO.Event;

using Common.DTO;

public sealed class GetEventsPaginationRequestDTO : PaginationDTO
{
    public string Search { get; set; }
}