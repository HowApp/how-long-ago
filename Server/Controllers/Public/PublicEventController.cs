namespace How.Server.Controllers.Public;

using Common.Constants;
using Common.ResultType;
using Core.DTO.Public.Event;
using Core.Services.Public.PublicEvent;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiExplorerSettings(GroupName = SwaggerDocConstants.Public)]
public class PublicEventController : BaseController
{
    private readonly IPublicEventService _eventService;

    public PublicEventController(IPublicEventService eventService)
    {
        _eventService = eventService;
    }
    
    [HttpGet]
    [SwaggerOperation("Get Events list with pagination")]
    [ProducesResponseType<Result<GetEventsPaginationPublicResponseDTO>>(200)]
    [Route("api/public/event/list-pagination")]
    public async Task<IActionResult> GetEventsPagination([FromQuery] GetEventsPaginationPublicRequestDTO publicRequest)
    {
        var result = await _eventService.GetEventsPagination(publicRequest);

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get Event detail by id")]
    [ProducesResponseType<Result<GetEventByIdResponseDTO>>(200)]
    [Route("api/public/event/{eventId:int:min(1)}/details")]
    public async Task<IActionResult> GetEventById([FromRoute] int eventId)
    {
        var result = await _eventService.GetEventById(eventId);

        return HttpResult(result);
    }
}