namespace How.Server.Controllers.Public;

using Common.Constants;
using Common.ResultType;
using Core.DTO.Record;
using Core.Services.Public.PublicRecord;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiExplorerSettings(GroupName = SwaggerDocConstants.Public)]
public class PublicRecordController : BaseController
{
    private readonly IPublicRecordService _publicRecordService;

    public PublicRecordController(IPublicRecordService publicRecordService)
    {
        _publicRecordService = publicRecordService;
    }

    [HttpGet]
    [SwaggerOperation("Get Record list with pagination")]
    [ProducesResponseType<Result<GetRecordsPaginationResponseDTO>>(200)]
    [Route("api/public/event/{eventId:int:min(1)}/record/list-pagination")]
    public async Task<IActionResult> GetRecordsPagination(
        [FromRoute] int eventId,
        [FromQuery] GetRecordsPaginationRequestDTO request)
    {
        var result = await _publicRecordService.GetRecordsPagination(eventId, request);

        return HttpResult(result);
    }
}