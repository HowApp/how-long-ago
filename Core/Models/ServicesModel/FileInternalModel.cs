namespace How.Core.Models.ServicesModel;

public class FileInternalModel
{
    public string Hash { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public long Size { get; set; }
    
    // TODO remove after setup cloud storage
    public byte[] Content { get; set; } = [];
}