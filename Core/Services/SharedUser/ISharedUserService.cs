namespace How.Core.Services.SharedUser;

using Common.ResultType;
using DTO.SharedUser;

public interface ISharedUserService
{
    Task<Result<int>> CreateSharedUser(CreateSharedUserRequestDTO request);
    Task<Result<GetSharedUsersResponseDTO>> GetSharedUsers();
}