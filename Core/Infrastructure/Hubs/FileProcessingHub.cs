namespace How.Core.Infrastructure.Hubs;

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

public class FileProcessingHub : Hub
{
    private readonly ILogger<FileProcessingHub> _logger;
    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        
            Console.WriteLine($"User {userId} is connected");
        
            await base.OnConnectedAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}