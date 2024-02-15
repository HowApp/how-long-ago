namespace How.Shared.DTO.Auth;

public class CurrentUserDTO
{
    public bool IsAuthenticate { get; set; }
    public string UserName { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}