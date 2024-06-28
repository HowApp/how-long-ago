namespace How.Core.DTO.SharedUser;

using FluentValidation;

public class CreateSharedUserRequestDTOValidator : AbstractValidator<CreateSharedUserRequestDTO>
{
    public CreateSharedUserRequestDTOValidator()
    {
        RuleFor(r => r.UserId)
            .GreaterThan(0)
            .WithMessage("Provide correct USer ID!");
    }
}