namespace How.Core.Database.TemporaryStorageEntity;

using Entities.Base;

public class File : PKey
{
    public string Name { get; set; }
    public byte[] Content { get; set; } = [];
}