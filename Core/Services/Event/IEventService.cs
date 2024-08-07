namespace How.Core.Services.Event;

using Common.ResultType;
using DTO.Dashboard.Event;
using Infrastructure.Enums;

public interface IEventService
{
    Task<Result<int>> CreateEvent(CreateEventRequestDTO request);
    Task<Result> UpdateActivateEventStatus(int eventId, bool setActive);
    Task<Result> UpdateEventAccess(int eventId, bool setPublic);
    Task<Result> UpdateEvent(int eventId, UpdateEventRequestDTO request);
    Task<Result<UpdateEventImageResponseDTO>> UpdateEventImage(int eventId, UpdateEventImageRequestDTO request);
    Task<Result<GetEventsPaginationResponseDTO>> GetEventsPagination(GetEventsPaginationRequestDTO request, FilterType filterType = FilterType.IncludeCreatedBy);
    Task<Result> DeleteEvent(int eventId);
}