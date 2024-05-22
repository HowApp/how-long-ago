namespace How.Core.DTO.Record;

using FluentValidation;

public class CreateRecordRequestDTOValidator : AbstractValidator<CreateRecordRequestDTO>
{
    public CreateRecordRequestDTOValidator()
    {
        RuleFor(r => r.Description)
            .NotEmpty()
            .MaximumLength(2048)
            .WithMessage("Provide record Description!");
    }
}