namespace How.Core.Services.CurrentUser;

using System.Security.Claims;

public interface ICurrentUserService
{
    ClaimsPrincipal User { get; }
    int UserId { get; }
}