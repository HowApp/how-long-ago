namespace How.Core.DTO.Dashboard.Event;

using Common.DTO;
using FluentValidation;

public class GetEventsPaginationRequestDTOValidation : AbstractValidator<GetEventsPaginationRequestDTO>
{
    public GetEventsPaginationRequestDTOValidation()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0!");
        
        RuleFor(r => r.Size)
            .LessThan(PaginationDTO.MaxSize)
            .WithMessage($"Page Size must be less than {PaginationDTO.MaxSize}!");

        RuleFor(r => r.Search)
            .MaximumLength(100)
            .WithMessage("Too long search request!");

        RuleFor(r => r.Status)
            .IsInEnum()
            .WithMessage("Provide correct Event Status!");
        
        RuleFor(r => r.Access)
            .IsInEnum()
            .WithMessage("Provide correct Event Active status!");
    }
}