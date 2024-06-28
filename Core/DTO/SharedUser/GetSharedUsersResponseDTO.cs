namespace How.Core.DTO.SharedUser;

using Models;

public sealed class GetSharedUsersResponseDTO
{
    public List<UserInfoModelLongDTO> Users { get; set; }
}