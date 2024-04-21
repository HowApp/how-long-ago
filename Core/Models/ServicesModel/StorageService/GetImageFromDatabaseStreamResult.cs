namespace How.Core.Models.ServicesModel.StorageService;

public class GetImageFromDatabaseStreamResult
{
    public string MimeType { get; set; }
    public Stream Content { get; set; }
}