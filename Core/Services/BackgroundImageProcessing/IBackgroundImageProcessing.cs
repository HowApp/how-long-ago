namespace How.Core.Services.BackgroundImageProcessing;

public interface IBackgroundImageProcessing
{
    Task RecordImageProcessing(
        int userId,
        int recordId,
        int[] fileIds);
}