namespace How.Core.Database.Entities.Storage;

using Base;

public class AppFile : BaseIdentityKey
{
    public string FileHash { get; set; }
    public string FileName { get; set; }
    public string Extension { get; set; }
    public long FileSize { get; set; }
    
    public byte[] Content { get; set; }
}