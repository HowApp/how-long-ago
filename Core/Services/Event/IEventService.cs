namespace How.Core.Services.Event;

using Common.ResultType;
using DTO.Event;

public interface IEventService
{
    Task<Result<int>> CreateEvent(CreateEventRequestDTO request);
    Task<Result> ActivateEvent(int eventId);
    Task<Result> DeactivateEvent(int eventId);
    Task<Result> UpdateEvent(int eventId, UpdateEventRequestDTO request);
    Task<Result<UpdateEventImageResponseDTO>> UpdateEventImage(int eventId, UpdateEventImageRequestDTO request);
    Task<Result<GetEventsPaginationResponseDTO>> GetEventsPagination(GetEventsPaginationRequestDTO request);
    Task<Result> DeleteEvent(int eventId);
}