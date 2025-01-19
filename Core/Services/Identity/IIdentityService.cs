namespace How.Core.Services.Identity;

using Common.ResultType;
using DTO.Identity;

public interface IIdentityService
{
    Result<CurrentUserResponseDTO> GetCurrentUserInfo();
}