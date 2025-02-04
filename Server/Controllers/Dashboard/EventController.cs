namespace How.Server.Controllers.Dashboard;

using Common.Constants;
using Common.ResultType;
using Core.DTO.Dashboard.Event;
using Core.Infrastructure.Enums;
using Core.Services.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Authorize]
[ApiExplorerSettings(GroupName = SwaggerDocConstants.Dashboard)]
public class EventController : BaseController
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost]
    [SwaggerOperation("Create Event, return ID")]
    [ProducesResponseType<Result<int>>(200)]
    [Route("api/dashboard/event/create")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequestDTO request)
    {
        var result = await _eventService.CreateEvent(request);

        return HttpResult(result);
    }

    [HttpGet]
    [SwaggerOperation("Get own Events list with pagination")]
    [ProducesResponseType<Result<GetEventsPaginationResponseDTO>>(200)]
    [Route("api/dashboard/event/list-pagination/own")]
    public async Task<IActionResult> GetOwnEventsPagination([FromQuery] GetEventsPaginationRequestDTO request)
    {
        var result = await _eventService.GetEventsPagination(request);

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get own Event by ID")]
    [ProducesResponseType<Result<GetEventByIdResponseDTO>>(200)]
    [Route("api/dashboard/event/{eventId:int:min(1)}/own")]
    public async Task<IActionResult> GetOwnEventById([FromRoute] int eventId)
    {
        var result = await _eventService.GetEventById(eventId);

        return HttpResult(result);
    }

    [HttpGet]
    [SwaggerOperation("Get shared Events list with pagination")]
    [ProducesResponseType<Result<GetEventsPaginationResponseDTO>>(200)]
    [Route("api/dashboard/event/list-pagination/shared")]
    public async Task<IActionResult> GetSharedEventsPagination([FromQuery] GetEventsPaginationRequestDTO request)
    {
        var result = await _eventService.GetEventsPagination(request, InternalAccessFilter.IncludeShared);

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get shared Event by ID")]
    [ProducesResponseType<Result<GetEventByIdResponseDTO>>(200)]
    [Route("api/dashboard/event/{eventId:int:min(1)}/shared")]
    public async Task<IActionResult> GetSharedEventById([FromRoute] int eventId)
    {
        var result = await _eventService.GetEventById(eventId, InternalAccessFilter.IncludeShared);

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get public active Events list with pagination")]
    [ProducesResponseType<Result<GetEventsPaginationResponseDTO>>(200)]
    [Route("api/dashboard/event/list-pagination/public-active")]
    public async Task<IActionResult> GetPublicActiveEventsPagination([FromQuery] GetEventsPaginationRequestDTO request)
    {
        var result = await _eventService.GetEventsPagination(request, InternalAccessFilter.None);

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get public active Event by ID")]
    [ProducesResponseType<Result<GetEventByIdResponseDTO>>(200)]
    [Route("api/dashboard/event/{eventId:int:min(1)}/public-active")]
    public async Task<IActionResult> GetPublicActiveEventById([FromRoute] int eventId)
    {
        var result = await _eventService.GetEventById(
            eventId,
            InternalAccessFilter.None,
            EventStatusFilter.Active,
            EventAccessFilter.Public);

        return HttpResult(result);
    }

    [HttpPatch]
    [SwaggerOperation("Activate/Deactivate Event")]
    [ProducesResponseType<Result>(200)]
    [Route("api/dashboard/event/{id:int:min(1)}/activate-status")]
    public async Task<IActionResult> Activate(
        [FromRoute] int id,
        [FromQuery] bool setActive)
    {
        var result = await _eventService.UpdateActivateEventStatus(id, setActive);

        return HttpResult(result);
    }

    [HttpPatch]
    [SwaggerOperation("Set event Access status Public/private")]
    [ProducesResponseType<Result>(200)]
    [Route("api/dashboard/event/{id:int:min(1)}/access-status")]
    public async Task<IActionResult> UpdateAccessStatus(
        [FromRoute] int id,
        [FromQuery] bool setPublic)
    {
        var result = await _eventService.UpdateEventAccess(id, setPublic);

        return HttpResult(result);
    }

    [HttpPatch]
    [SwaggerOperation("Update event")]
    [ProducesResponseType<Result>(200)]
    [Route("api/dashboard/event/{id:int:min(1)}/update")]
    public async Task<IActionResult> UpdateEvent(
        [FromRoute] int id,
        [FromForm] UpdateEventRequestDTO request)
    {
        var result = await _eventService.UpdateEvent(id, request);

        return HttpResult(result);
    }
    
    [HttpPatch]
    [SwaggerOperation("Update like event state")]
    [ProducesResponseType<Result<LikeState>>(200)]
    [Route("api/dashboard/event/{id:int:min(1)}/like")]
    public async Task<IActionResult> UpdateEventLikeState(
        [FromRoute] int id,
        [FromQuery] LikeState request)
    {
        var result = await _eventService.UpdateLikeState(id, request);

        return HttpResult(result);
    }
    

    [HttpPut]
    [SwaggerOperation("Update event image, returning hash")]
    [ProducesResponseType<Result<UpdateEventImageResponseDTO>>(200)]
    [Route("api/dashboard/event/{id:int:min(1)}/image")]
    public async Task<IActionResult> UpdateEventImage(
        [FromRoute] int id,
        [FromForm] UpdateEventImageRequestDTO request)
    {
        var result = await _eventService.UpdateEventImage(id, request);

        return HttpResult(result);
    }

    [HttpDelete]
    [SwaggerOperation("Delete event")]
    [ProducesResponseType<Result>(200)]
    [Route("api/dashboard/event/{id:int:min(1)}/delete")]
    public async Task<IActionResult> DeleteEvent([FromRoute] int id)
    {
        var result = await _eventService.DeleteEvent(id);

        return HttpResult(result);
    }
    
    [HttpPost]
    [SwaggerOperation("Add Event to saved")]
    [ProducesResponseType<Result>(200)]
    [Route("api/dashboard/event/{id:int:min(1)}/saved/add")]
    public async Task<IActionResult> AddEventToSaved([FromRoute] int id)
    {
        var result = await _eventService.AddEventToSaved(id);

        return HttpResult(result);
    }
    
    [HttpDelete]
    [SwaggerOperation("Delete event")]
    [ProducesResponseType<Result>(200)]
    [Route("api/dashboard/event/{id:int:min(1)}/saved/delete")]
    public async Task<IActionResult> DeleteEventFromSaved([FromRoute] int id)
    {
        var result = await _eventService.DeleteEventFromSaved(id);

        return HttpResult(result);
    }
    
    // TODO add ability to get saved events
}