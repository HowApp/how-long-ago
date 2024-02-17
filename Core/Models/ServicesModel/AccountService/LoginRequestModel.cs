namespace How.Core.Models.ServicesModel.AccountService;

public class LoginRequestModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}