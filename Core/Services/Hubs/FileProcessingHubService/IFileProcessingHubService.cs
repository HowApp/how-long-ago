namespace How.Core.Services.Hubs.FileProcessingHubService;

public interface IFileProcessingHubService
{
    Task NotifyUSer(string message);
}