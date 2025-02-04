namespace How.Core.Services.Hubs.FileProcessingHubService;

public interface IFileProcessingHubService
{
    Task NotifyUser(int userId, string message);
    Task NotifyCurrentUser(string message);
}