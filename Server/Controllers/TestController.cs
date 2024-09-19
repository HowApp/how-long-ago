namespace How.Server.Controllers;

using Common.ResultType;
using Core.Infrastructure.Hubs;
using Core.Services.CurrentUser;
using Core.Services.Hubs.FileProcessingHubService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NodaTime;
using Swashbuckle.AspNetCore.Annotations;

[Authorize]
public class TestController : BaseController
{
    private readonly IFileProcessingHubService _fileProcessing;
    
    public TestController(IFileProcessingHubService fileProcessing)
    {
        _fileProcessing = fileProcessing;
    }

    [HttpPost]
    [SwaggerOperation("Send event to user")]
    [ProducesResponseType<Result>(200)]
    [Route("api/test/event/create")]
    public async Task<IActionResult> TestSignalR()
    {
        _fileProcessing.NotifyUser("Your file has been processed.");
        return HttpResult(Result.Success());
    }
    
    
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