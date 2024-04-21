namespace How.Core.Models.ServicesModel.StorageService;

public class GetImageFromDatabaseByteResult
{
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public byte[] Content { get; set; }
}