namespace How.Core.DTO.Record;

using FluentValidation;

public class CreateRecordRequestDTOValidator : AbstractValidator<CreateRecordRequestDTO>
{
    public CreateRecordRequestDTOValidator()
    {
        RuleFor(r => r.Description)
            .NotEmpty()
            .WithMessage("Provide record Description!");
    }
}