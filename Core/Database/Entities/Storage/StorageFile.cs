namespace How.Core.Database.Entities.Storage;

using Base;

public class StorageFile : IdentityKey
{
    public string Hash { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public long Size { get; set; }
    
    // TODO remove after setup cloud storage
    public byte[] Content { get; set; } = [];
}