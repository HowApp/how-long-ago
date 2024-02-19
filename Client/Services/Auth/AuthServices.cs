namespace How.Client.Services.Auth;

using System.Net;
using System.Net.Http.Json;
using How.Shared.DTO.Auth;
using Newtonsoft.Json;
using ResultClient;

public class AuthServices : IAuthServices
{
    private readonly HttpClient _httpClient;

    public AuthServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task Login(LoginRequestDTO request)
    {
        var result = await _httpClient.PostAsJsonAsync("api/Account/Login", request);

        if (result.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new Exception(await result.Content.ReadAsStringAsync());
        }

        result.EnsureSuccessStatusCode();
    }

    public async Task Register(RegisterRequestDTO request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Account/Register", request);

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            throw new Exception(await response.Content.ReadAsStringAsync());
        }
        
        var stringResult = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<ResultClient<RegisterResponseDTO>>(stringResult);
        
        if (result is not null)
        {
            if (result.Failed)
            {
                throw new Exception(string.Join(" ", result.Error.Values.ToList()));
            }
        }
        
        response.EnsureSuccessStatusCode();
    }

    public async Task Logout()
    {
        var result = await _httpClient.PostAsync("api/Account/Logout", null);
        result.EnsureSuccessStatusCode();
    }

    public async Task<CurrentUser> CurrentUserInfo()
    {
        var response = await _httpClient.GetFromJsonAsync<ResultClient<CurrentUserResponseDTO>>("api/Account/CurrentUserInfo");
        
        if (response is null)
        {
            return new CurrentUser();
        }

        if (response.Failed)
        {
            throw new Exception(response.Error?.ToString());
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