namespace How.Core.DTO.RecordImage;

using FluentValidation;

public class UpdateRecordImagesRequestDTOValidator : AbstractValidator<UpdateRecordImagesRequestDTO>
{
    public UpdateRecordImagesRequestDTOValidator()
    {
        RuleFor(r => r.ImageIds)
            .Must(arr => arr.Distinct().Count() == arr.Length)
            .WithMessage("Provide sequence without duplicates!");
        
        RuleForEach(r => r.ImageIds)
            .Must(id => id > 0)
            .WithMessage("Provide correct Ids sequence!");
    }
}