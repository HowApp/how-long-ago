namespace How.Core.Services.Account;

using Common.ResultType;
using DTO.Account;

public interface IAccountService
{
    Task<Result<GetUserInfoResponseDTO>> GetUserInfo();
}