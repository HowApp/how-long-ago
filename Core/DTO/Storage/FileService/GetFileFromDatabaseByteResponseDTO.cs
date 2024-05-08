namespace How.Core.DTO.Storage.FileService;

public sealed class GetFileFromDatabaseByteResponseDTO
{
    public string FileName { get; set; }
    public string MimeType { get; set; }
    public byte[] Content { get; set; }
}