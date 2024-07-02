namespace How.Core.DTO.Public.Event;

using Common.DTO;

public sealed class GetEventsPaginationRequestDTO : PaginationDTO
{
    public string Search { get; set; } = string.Empty;
}