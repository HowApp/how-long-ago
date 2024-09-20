namespace How.Core.Services.BackgroundImageProcessing;

public interface IBackgroundImageProcessing
{
    Task RecordImageProcessing(
        int userId,
        int recordId,
        List<(string name, byte[] content)> files);
}