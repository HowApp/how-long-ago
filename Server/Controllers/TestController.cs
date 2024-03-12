namespace How.Server.Controllers;

using Common.ResultType;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Test;

public class TestController : BaseController
{
    [Route("api/Test/create")]
    [HttpPost]
    public async Task<IActionResult> Create(TestPostRequestDTO request)
    {
        var result = Result.Success();
        
        return HttpResult(result);
    }
}