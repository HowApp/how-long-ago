namespace How.Core.Services.BackgroundImageProcessing;

public interface IBackgroundImageProcessing
{
    Task RecordImageProcessing(
        int userId,
        int recordId,
        int[] fileIds);
    
    Task EventImageProcessing(
        int userId,
        int eventId,
        int fileId);
}