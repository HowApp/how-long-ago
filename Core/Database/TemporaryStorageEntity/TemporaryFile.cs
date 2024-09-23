namespace How.Core.Database.TemporaryStorageEntity;

using Entities.Base;

public class TemporaryFile : PKey
{
    public string Name { get; set; }
    public byte[] Content { get; set; } = [];
}