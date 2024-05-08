namespace How.Core.Services.Identity;

using Common.ResultType;
using DTO.Identity;

public interface IIdentityService
{
    Task<Result> Login(LoginRequestDTO requestModel);
    Task<Result> Register(RegisterRequestDTO requestModel);
    Task<Result> Logout();
    Task<Result<CurrentUserResponseDTO>> GetCurrentUserInfo();
}