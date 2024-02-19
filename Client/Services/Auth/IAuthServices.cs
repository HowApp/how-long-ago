namespace How.Client.Services.Auth;

using How.Shared.DTO.Auth;

public interface IAuthServices
{
    Task Login(LoginRequestDTO request);
    Task Register(RegisterRequestDTO request);
    Task Logout();
    Task<CurrentUser> CurrentUserInfo();
}