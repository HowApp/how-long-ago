namespace How.Core.Services.AccountServices;

using Common.ResultClass;
using Models.ServicesModel.AccountService;

public interface IAccountService
{
    Task<Result> Login(LoginRequestModel requestModel);
    Task<Result<RegisterResponseModel>> Register(RegisterRequestModel requestModel);
    Task<Result> Logout();
    Task<Result<CurrentUserResponseModel>> GetCurrentUserInfo();
}