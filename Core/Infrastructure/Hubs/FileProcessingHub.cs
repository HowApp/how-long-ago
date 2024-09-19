namespace How.Core.Infrastructure.Hubs;

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

public class FileProcessingHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        
        Console.WriteLine($"User {userId} is connected");
        
        await base.OnConnectedAsync();
    }
}