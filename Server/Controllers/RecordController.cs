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
    public async Task<IActionResult> CreateRecord(
        [FromRoute] int eventId,
        [FromBody] CreateRecordRequestDTO request)
    {
        var result = await _recordService.CreateRecord(eventId, request);

        return HttpResult(result);
    }
    
    [HttpGet]
    [SwaggerOperation("Get Record list with pagination")]
    [ProducesResponseType<Result<GetRecordsPaginationResponseDTO>>(200)]
    [Route("api/event/{eventId:int:min(1)}/record/list-pagination")]
    public async Task<IActionResult> GetEventsPagination(
        int eventId,
        [FromQuery] GetRecordsPaginationRequestDTO request)
    {
        var result = await _recordService.GetRecordsPagination(eventId, request);

        return HttpResult(result);
    }
    
    [HttpPatch]
    [SwaggerOperation("Update record")]
    [ProducesResponseType<Result>(200)]
    [Route("api/event/record/{recordId:int:min(1)}/update")]
    public async Task<IActionResult> UpdateRecord(
        [FromRoute] int recordId,
        [FromBody] UpdateRecordRequestDTO request)
    {
        var result = await _recordService.UpdateRecord(recordId, request);

        return HttpResult(result);
    }
    
    [HttpPost]
    [SwaggerOperation("Create record images, returning hashes")]
    [ProducesResponseType<Result<CreateRecordImagesResponseDTO>>(200)]
    [Route("api/event/record/{recordId:int:min(1)}/image/create")]
    public async Task<IActionResult> CreateRecordImage(
        [FromRoute] int recordId,
        [FromForm] CreateRecordImagesRequestDTO request)
    {
        var result = await _recordService.CreateRecordImages(recordId, request);

        return HttpResult(result);
    }
    
    [HttpPatch]
    [SwaggerOperation("Update record images")]
    [ProducesResponseType<Result<CreateRecordImagesResponseDTO>>(200)]
    [Route("api/event/record/{recordId:int:min(1)}/image/update")]
    public async Task<IActionResult> UpdateRecordImages(
        [FromRoute] int recordId,
        [FromBody] UpdateRecordImagesRequestDTO request)
    {
        var result = await _recordService.UpdateRecordImages(recordId, request);

        return HttpResult(result);
    }
}