namespace How.Core.Services.Public.PublicEvent;

using Common.ResultType;
using DTO.Public.Event;

public interface IPublicEventService
{
    Task<Result<GetEventsPaginationResponseDTO>> GetEventsPagination(GetEventsPaginationPublicRequestDTO publicRequest);
}