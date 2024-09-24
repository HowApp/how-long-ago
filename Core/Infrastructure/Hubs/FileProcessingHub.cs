namespace How.Core.Infrastructure.Hubs;

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

public class FileProcessingHub : Hub
{
    private readonly ILogger<FileProcessingHub> _logger;

    public FileProcessingHub(ILogger<FileProcessingHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} was connected");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "unauthorized");
                Console.WriteLine($"Unauthorized User was connected");
            }

            await base.OnConnectedAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}