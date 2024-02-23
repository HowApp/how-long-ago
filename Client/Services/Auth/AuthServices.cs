namespace How.Client.Services.Auth;

using ClientAPI;
using How.Shared.DTO.Auth;
using ResultClient;

public class AuthServices : ClientAPI, IAuthServices
{
    public AuthServices(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task Login(LoginRequestDTO request)
    {
        await PostAsync<ResultResponse, LoginRequestDTO>("api/account/login", request);
    }

    public async Task Register(RegisterRequestDTO request)
    {
        await PostAsync<ResultResponse, RegisterRequestDTO>("api/account/register", request);
    }

    public async Task Logout()
    {
        await PostAsync<ResultResponse>("api/account/logout");
    }

    public async Task<CurrentUser> CurrentUserInfo()
    {
        var response = await GetAsync<ResultResponse<CurrentUserResponseDTO>>("api/account/current-user-info");

        if (response.Data is null)
        {
            return new CurrentUser();
        }
        
        var result = new CurrentUser
        {
            IsAuthenticate = response.Data.IsAuthenticate,
            UserName = response.Data.UserName,
            Claims = response.Data.Claims
        };
        
        return result;
    }
}