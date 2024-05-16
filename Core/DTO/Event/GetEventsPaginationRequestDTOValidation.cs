namespace How.Core.DTO.Event;

using FluentValidation;

public class GetEventsPaginationRequestDTOValidation : AbstractValidator<GetEventsPaginationRequestDTO>
{
    public GetEventsPaginationRequestDTOValidation()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0!");
        
        RuleFor(r => r.Size)
            .GreaterThan(0)
            .WithMessage("Page Size must be greater than 0!");

        RuleFor(r => r.Search)
            .MaximumLength(100)
            .WithMessage("Too long search request!");
    }
}