namespace How.Core.DTO.Account;

using FluentValidation;

public class GetUserInfoByUserNameRequestDTOValidator : AbstractValidator<GetUserInfoByUserNameRequestDTO>
{
    public GetUserInfoByUserNameRequestDTOValidator()
    {
        RuleFor(r => r.Search)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Provide user Name!")
            .MaximumLength(512)
            .WithMessage("Search prompt too long!");
    }
}