namespace How.Server.Controllers;

using Common.ResultType;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Annotations;

public class TestController : BaseController
{
    [HttpGet]
    [SwaggerOperation("Create Event, return ID")]
    [ProducesResponseType<Result>(200)]
    [Route("api/test/event/create")]
    public async Task<IActionResult> CreateEvent([FromQuery] TestRequestDTO request)
    {

        return HttpResult(Result.Success());
    }
}


public class TestRequestDTO
{
    public int Age { get; set; }
    public LocalDate Date { get; set; }
}