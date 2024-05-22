namespace How.Core.DTO.Record;

using FluentValidation;

public class UpdateRecordRequestDTOValidator : AbstractValidator<UpdateRecordRequestDTO>
{
    public UpdateRecordRequestDTOValidator()
    {
        RuleFor(r => r.Description)
            .NotEmpty()
            .WithMessage("Provide record Description!")
            .MaximumLength(2048)
            .WithMessage("Description too long!");
    }
}