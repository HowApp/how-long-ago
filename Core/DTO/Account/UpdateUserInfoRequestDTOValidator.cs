namespace How.Core.DTO.Account;

using FluentValidation;

public class UpdateUserInfoRequestDTOValidator : AbstractValidator<UpdateUserInfoRequestDTO>
{
    public UpdateUserInfoRequestDTOValidator()
    {
        RuleFor(r => r.FirstName)
            .NotEmpty()
            .WithMessage("Provide first name!")
            .MaximumLength(2048)
            .WithMessage("First name too long!");
        
        RuleFor(r => r.LastName)
            .NotEmpty()
            .WithMessage("Provide last name!")
            .MaximumLength(2048)
            .WithMessage("First name too long!");
    }
}