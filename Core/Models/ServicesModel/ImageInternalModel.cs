namespace How.Core.Models.ServicesModel;

public class ImageInternalModel
{
    public int ImageHeight { get; set; }
    public int ImageWidth { get; set; }
    
    public int ThumbnailHeight { get; set; }
    public int ThumbnailWidth { get; set; }
    
    public FileInternalModel Main { get; set; }
    public FileInternalModel Thumbnail { get; set; }
}