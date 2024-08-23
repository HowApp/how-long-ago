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
    Task<Result<LikeState>> UpdateLikeState(int eventId, LikeState likeState);
    Task<Result<GetEventsPaginationResponseDTO>> GetEventsPagination(GetEventsPaginationRequestDTO request, AccessFilterType accessFilterType = AccessFilterType.IncludeCreatedBy);
    Task<Result> DeleteEvent(int eventId);
    Task<Result> AddEventToSaved(int eventId);
    Task<Result> DeleteEventFromSaved(int eventId);
}