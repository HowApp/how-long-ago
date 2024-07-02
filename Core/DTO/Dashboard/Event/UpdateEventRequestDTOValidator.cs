namespace How.Core.DTO.Dashboard.Event;

using FluentValidation;

public class UpdateEventRequestDTOValidator : AbstractValidator<UpdateEventRequestDTO>
{
    public UpdateEventRequestDTOValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage("Provide Name!")
            .MaximumLength(1024)
            .WithMessage("Name too long!");;
    }
}