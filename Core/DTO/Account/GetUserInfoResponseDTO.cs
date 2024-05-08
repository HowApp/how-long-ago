namespace How.Core.DTO.Account;

using Models;

public sealed class GetUserInfoResponseDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ImageModel Image { get; set; }
}