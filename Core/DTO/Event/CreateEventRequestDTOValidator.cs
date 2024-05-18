namespace How.Core.DTO.Event;

using FluentValidation;

public class CreateEventRequestDTOValidator : AbstractValidator<CreateEventRequestDTO>
{
    public CreateEventRequestDTOValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage("Provide event Name!");
    }
}