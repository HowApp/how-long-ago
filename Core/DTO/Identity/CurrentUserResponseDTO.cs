namespace How.Core.DTO.Identity;

public sealed class CurrentUserResponseDTO
{
    public bool IsAuthenticate { get; set; }
    public string UserName { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}