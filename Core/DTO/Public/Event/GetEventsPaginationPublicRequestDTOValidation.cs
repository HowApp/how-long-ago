namespace How.Core.DTO.Public.Event;

using Common.DTO;
using FluentValidation;

public class GetEventsPaginationPublicRequestDTOValidation : AbstractValidator<GetEventsPaginationPublicRequestDTO>
{
    public GetEventsPaginationPublicRequestDTOValidation()
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
    }
}