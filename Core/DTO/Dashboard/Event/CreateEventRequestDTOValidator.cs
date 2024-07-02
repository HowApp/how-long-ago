namespace How.Core.DTO.Dashboard.Event;

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