namespace How.Server.Controllers;

using Common.ResultType;
using Core.CQRS.Queries.Test;
using Core.DTO.Test;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class TestController : BaseController
{
    private readonly ISender _sender;

    public TestController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Route("api/test/create")]
    public async Task<IActionResult> Create(TestPostRequestDTO request)
    {
        var result = Result.Success();
        
        return HttpResult(result);
    }
    
    [HttpGet]
    [Route("api/test/get")]
    public async Task<IActionResult> Get([FromQuery]TestPostRequestDTO request)
    {
        var query = new TestQuery
        {
            Number = request.Id
        };
        var result = await _sender.Send(query);
        
        return HttpResult(result);
    }

    [HttpPost]
    [Route("api/test/upload-image")]
    public async Task<IActionResult> UploadImage([FromForm] TestPostImageRequestDTO file)
    {
        return Ok();
    }
}