namespace How.Core.Services.Event;

using Common.ResultType;
using DTO.Event;

public interface IEventService
{
    Task<Result<int>> CreateEvent(CreateEventRequestDTO request);
    Task<Result> ActivateEvent(int eventId);
    Task<Result<GetEventsPaginationResponseDTO>> GetEventsPagination(GetEventsPaginationRequestDTO request);
}