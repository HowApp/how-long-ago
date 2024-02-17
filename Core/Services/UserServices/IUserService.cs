namespace How.Core.Services.UserServices;

using System.Security.Claims;

public interface IUserService
{
    ClaimsPrincipal User { get; }
}