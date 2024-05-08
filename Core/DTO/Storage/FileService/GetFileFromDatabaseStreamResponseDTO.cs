namespace How.Core.DTO.Storage.FileService;

public sealed class GetFileFromDatabaseStreamResponseDTO
{
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public Stream Content { get; set; }
}