namespace How.Core.Models.ServicesModel.AccountService;

public class RegisterResponseModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}