namespace How.Client.Services.Provider;

using System.Security.Claims;
using Auth;
using How.Shared.DTO.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

public class CustomStateProvider : AuthenticationStateProvider
{
    private readonly IAuthServices _apiAuthServices;
    private CurrentUser _currentUser;

    public CustomStateProvider(IAuthServices authServices)
    {
        _apiAuthServices = authServices;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();

        try
        {
            var userInfo = await GetCurrentUser();
            if (userInfo.IsAuthenticate)
            {
                var claims =
                    new[] { new Claim(ClaimTypes.Name, _currentUser.UserName) }
                        .Concat(_currentUser.Claims.Select(c => new Claim(c.Key, c.Value)));

                // identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                identity = new ClaimsIdentity(claims,"Server authentication");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request failed: {e}");
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task Logout()
    {
        await _apiAuthServices.Logout();
        _currentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Login(LoginRequestDTO request)
    {
        await _apiAuthServices.Login(request);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Register(RegisterRequestDTO request)
    {
        await _apiAuthServices.Register(request);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    private async Task<CurrentUser> GetCurrentUser()
    {
        if (_currentUser is not null && _currentUser.IsAuthenticate) 
        {
            return _currentUser;
        }

        _currentUser = await _apiAuthServices.CurrentUserInfo();

        return _currentUser;
    }
}