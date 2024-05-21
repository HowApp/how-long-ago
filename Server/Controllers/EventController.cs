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
    [ProducesResponseType<Result<int>>(200)]
    [Route("api/event/{id:int:min(1)}/activate")]
    public async Task<IActionResult> Activate([FromRoute] int id)
    {
        var result = await _eventService.ActivateEvent(id);

        return HttpResult(result);
    }
}