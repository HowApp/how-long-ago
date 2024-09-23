namespace How.Core.Services.Hubs.FileProcessingHubService;

using CurrentUser;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

public class FileProcessingHubService : IFileProcessingHubService
{
    private readonly IHubContext<FileProcessingHub> _hubContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<FileProcessingHubService> _logger;

    public FileProcessingHubService(
        IHubContext<FileProcessingHub> hubContext,
        ICurrentUserService currentUserService,
        ILogger<FileProcessingHubService> logger)
    {
        _hubContext = hubContext;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task NotifyCurrentUser(string message)
    {
        try
        {
            await _hubContext.Clients
                .Group(_currentUserService.UserId.ToString())
                .SendAsync("ReceiveMessage", message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
    
    public async Task NotifyUser(int userId, string message)
    {
        try
        {
            await _hubContext.Clients
                .Group(userId.ToString())
                .SendAsync("ReceiveMessage", message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}