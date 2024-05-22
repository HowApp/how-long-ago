namespace How.Server.Controllers;

using Common.ResultType;
using Core.DTO.Event;
using Core.Services.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Authorize]
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
    [Route("api/event/create")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequestDTO request)
    {
        var result = await _eventService.CreateEvent(request);

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get Events list with pagination")]
    [ProducesResponseType<Result<GetEventsPaginationResponseDTO>>(200)]
    [Route("api/event/list-pagination")]
    public async Task<IActionResult> GetEventsPagination([FromQuery] GetEventsPaginationRequestDTO request)
    {
        var result = await _eventService.GetEventsPagination(request);

        return HttpResult(result);
    }
    
    [HttpPatch]
    [SwaggerOperation("Activate Event")]
    [ProducesResponseType<Result>(200)]
    [Route("api/event/{id:int:min(1)}/activate")]
    public async Task<IActionResult> Activate([FromRoute] int id)
    {
        var result = await _eventService.ActivateEvent(id);

        return HttpResult(result);
    }
    
    [HttpPatch]
    [SwaggerOperation("Deactivate Event")]
    [ProducesResponseType<Result>(200)]
    [Route("api/event/{id:int:min(1)}/deactivate")]
    public async Task<IActionResult> Deactivate([FromRoute] int id)
    {
        var result = await _eventService.DeactivateEvent(id);

        return HttpResult(result);
    }
    
    [HttpPatch]
    [SwaggerOperation("Update event")]
    [ProducesResponseType<Result>(200)]
    [Route("api/event/{id:int:min(1)}/update")]
    public async Task<IActionResult> UpdateEvent(
        [FromRoute] int id,
        [FromForm] UpdateEventRequestDTO request)
    {
        var result = await _eventService.UpdateEvent(id, request);

        return HttpResult(result);
    }
    
    [HttpPut]
    [SwaggerOperation("Update event image, returning hash")]
    [ProducesResponseType<Result<UpdateEventImageResponseDTO>>(200)]
    [Route("api/event/{id:int:min(1)}/image")]
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
    [Route("api/event/{id:int:min(1)}/delete")]
    public async Task<IActionResult> DeleteEvent([FromRoute] int id)
    {
        var result = await _eventService.DeleteEvent(id);

        return HttpResult(result);
    }
}