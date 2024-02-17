namespace How.Core.Models.ServicesModel.AccountService;

public class CurrentUserResponseModel
{
    public bool IsAuthenticate { get; set; }
    public string UserName { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}