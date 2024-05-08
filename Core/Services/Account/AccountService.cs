namespace How.Core.Services.Account;

using CurrentUser;
using MediatR;

public class AccountService : IAccountService
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public AccountService(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }
}