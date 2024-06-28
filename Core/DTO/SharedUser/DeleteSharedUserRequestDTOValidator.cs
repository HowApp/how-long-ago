namespace How.Core.DTO.SharedUser;

using FluentValidation;

public class DeleteSharedUserRequestDTOValidator : AbstractValidator<DeleteSharedUserRequestDTO>
{
    public DeleteSharedUserRequestDTOValidator()
    {
        RuleFor(r => r.SharedUserId)
            .GreaterThan(0)
            .WithMessage("Provide correct shared User ID!");
    }
}