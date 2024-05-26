namespace How.Server.Controllers;

using Common.ResultType;
using Core.DTO.Record;
using Core.DTO.RecordImage;
using Core.Services.Record;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Authorize]
public class RecordController : BaseController
{
    private readonly IRecordService _recordService;

    public RecordController(IRecordService recordService)
    {
        _recordService = recordService;
    }
    
    [HttpPost]
    [SwaggerOperation("Create Record, return ID")]
    [ProducesResponseType<Result<int>>(200)]
    [Route("api/event/{eventId:int:min(1)}/record/create")]
    public async Task<IActionResult> CreateEvent(
        [FromRoute] int eventId,
        [FromBody] CreateRecordRequestDTO request)
    {
        var result = await _recordService.CreateRecord(eventId, request);

        return HttpResult(result);
    }
    
    [HttpPatch]
    [SwaggerOperation("Update record")]
    [ProducesResponseType<Result>(200)]
    [Route("api/event/{eventId:int:min(1)}/record/{recordId:int:min(1)}/update")]
    public async Task<IActionResult> UpdateRecord(
        [FromRoute] int eventId,
        [FromRoute] int recordId,
        [FromForm] UpdateRecordRequestDTO request)
    {
        var result = await _recordService.UpdateRecord(eventId, recordId, request);

        return HttpResult(result);
    }
    
    [HttpPost]
    [SwaggerOperation("Create record images, returning hashes")]
    [ProducesResponseType<Result<CreateRecordImagesResponseDTO>>(200)]
    [Route("api/event/{eventId:int:min(1)}/record/{recordId:int:min(1)}/image")]
    public async Task<IActionResult> CreateEventImage(
        [FromRoute] int eventId,
        [FromRoute] int recordId,
        [FromForm] CreateRecordImagesRequestDTO request)
    {
        var result = await _recordService.CreateRecordImages(eventId, recordId, request);

        return HttpResult(result);
    }
}