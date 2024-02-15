namespace How.Server.Controllers;

using System.Net;
using Core.Database.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Auth;

[Route("api/[controller]/[action]")]
public class AccountController : BaseController
{
    private readonly UserManager<HowUser> _userManager;
    private readonly SignInManager<HowUser> _signInManager;

    public AccountController(
        UserManager<HowUser> userManager, 
        SignInManager<HowUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest("User not found!");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!signInResult.Succeeded)
        {
            return BadRequest("Invalid Password!");
        }

        await _signInManager.SignInAsync(user, request.RememberMe);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequestDTO request)
    {
        var user = new HowUser
        {
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(string.Join(';', result.Errors.Select(e =>e.Description)));
        }

        return await Login(
            new LoginRequestDTO
            {
                Email = request.Email,
                Password = request.Password,
                RememberMe = false
            });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CurrentUserInfo()
    {
        if (User.Identity is null)
        {
            return BadRequest();
        }
        
        return StatusCode(
            (int)HttpStatusCode.OK,
            new CurrentUserDTO
            {
                IsAuthenticate = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name ?? string.Empty,
                Claims = User.Claims.ToDictionary(c => c.Type, c => c.Value)
            }
        );
    }
}