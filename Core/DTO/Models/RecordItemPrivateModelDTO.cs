namespace How.Core.DTO.Models;

using Infrastructure.Enums;

public class RecordItemPrivateModelDTO : RecordItemPublicModelDTO
{
    public LikeState OwnLikeState { get; set; }
}