namespace How.Core.Services.Account;

using Common.ResultType;
using DTO.Account;

public interface IAccountService
{
    Task<Result<GetUserInfoResponseDTO>> GetUserInfo();
    Task<Result> UpdateUserInfo(UpdateUserInfoRequestDTO request);
    Task<Result<UpdateUserImageResponseDTO>> UpdateUserImage(UpdateUserImageRequestDTO request);
}