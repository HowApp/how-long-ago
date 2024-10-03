namespace How.Core.Services.Public.PublicEvent;

using Common.ResultType;
using DTO.Public.Event;

public interface IPublicEventService
{
    Task<Result<GetEventsPaginationPublicResponseDTO>> GetEventsPagination(
        GetEventsPaginationPublicRequestDTO publicRequest);
    Task<Result<GetEventPublicByIdResponseDTO>> GetEventById(int eventId);
}